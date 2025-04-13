using LAB1_WEB_API.Infrastructure.Generators.Data;
using LAB1_WEB_API.Interfaces.DataSaver;
using Neo4j.Driver;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace LAB1_WEB_API.Services.DataSavers;

public class Neo4jDataSaver : IDataSaver<GeneratedData>
{
    private readonly IDriver _neo4j;
    private readonly ILogger<Neo4jDataSaver> _logger;

    public Neo4jDataSaver(IDriver neo4j, ILogger<Neo4jDataSaver> logger)
    {
        _neo4j = neo4j;
        _logger = logger;
    }

    public async Task<SaveResult> SaveAsync(GeneratedData data)
    {
        _logger.LogInformation("Saving graph data to Neo4j...");
        await using var
            session = _neo4j.AsyncSession(o =>
                o.WithDatabase("neo4j")); // Используем using для автоматического закрытия сессии

        //Очистка графа перед вставкой (ОСТОРОЖНО!) - раскомментируйте, если нужно
        _logger.LogWarning("Detaching and deleting all nodes and relationships in Neo4j...");
        await session.ExecuteWriteAsync(async tx => { await tx.RunAsync("MATCH (n) DETACH DELETE n"); });
        _logger.LogInformation("Neo4j graph cleared.");


        // Создание узлов
        await session.ExecuteWriteAsync(async tx =>
        {
            _logger.LogDebug("Creating Neo4j Nodes...");
            if (data.Lectures.Any())
            {
                var lecturesParams = data.Lectures.Select(l => new Dictionary<string, object>
                    { { "id", l.Id }, { "name", l.Name }, { "req", l.Requirments } }).ToList();
                // *** ИСПРАВЛЕННАЯ СТРОКА ЗДЕСЬ ***
                await tx.RunAsync("UNWIND $batch AS props MERGE (n:Lecture {id: props.id}) SET n += props",
                    new { batch = lecturesParams });
            }

            if (data.Groups.Any())
            {
                // Эта часть была уже правильной
                var groupsParams = data.Groups.Select(g => new Dictionary<string, object>
                {
                    { "id", g.Id }, { "name", g.Name }, { "start", g.StartYear.ToString("yyyy-MM-dd") },
                    { "end", g.EndYear.ToString("yyyy-MM-dd") }
                }).ToList();
                await tx.RunAsync("UNWIND $batch AS props MERGE (g:Group {id: props.id}) SET g += props",
                    new { batch = groupsParams });
            }

            if (data.Students.Any())
            {
                // Эта часть была уже правильной
                var studentsParams = data.Students.Select(s => new Dictionary<string, object>
                        { { "id", s.Id }, { "fio", s.FullName }, { "dor", s.DateOfRecipient.ToString("yyyy-MM-dd") } })
                    .ToList();
                await tx.RunAsync("UNWIND $batch AS props MERGE (s:Student {id: props.id}) SET s += props",
                    new { batch = studentsParams });
            }

            _logger.LogDebug("Neo4j Nodes created/merged.");
        });

        // Создание связей HAS_LECTURE
        await session.ExecuteWriteAsync(async tx =>
        {
            _logger.LogDebug("Creating Neo4j HAS_LECTURE relationships...");
            if (data.Schedules.Any())
            {
                var scheduleParams = data.Schedules.Select(sch => new Dictionary<string, object>
                {
                    { "groupId", sch.GroupId }, { "lectureId", sch.LectureId },
                    { "start", sch.StartTime.ToString("o") }, { "end", sch.EndTime.ToString("o") }
                }).ToList();
                // Используем MERGE для связи, чтобы избежать дубликатов, если запускать повторно (хотя свойства перезапишутся)
                await tx.RunAsync(@"
                         UNWIND $batch AS props
                         MATCH (g:Group {id: props.groupId})
                         MATCH (l:Lecture {id: props.lectureId})
                         MERGE (g)-[r:HAS_LECTURE]->(l)
                         SET r.startTime = props.start, r.endTime = props.end",
                    new { batch = scheduleParams });
                _logger.LogDebug("Neo4j HAS_LECTURE relationships created/merged.");
            }
        });

        // Создание связей ATTENDED
        await session.ExecuteWriteAsync(async tx =>
        {
            _logger.LogDebug("Creating Neo4j ATTENDED relationships...");
            if (data.Visits.Any())
            {
                var scheduleLectureMap = data.Schedules.ToDictionary(s => s.Id, s => s.LectureId);
                var visitParams = data.Visits
                    .Where(v => scheduleLectureMap.ContainsKey(v.ScheduleId))
                    .Select(v => new Dictionary<string, object>
                    {
                        { "studentId", v.StudentId }, { "lectureId", scheduleLectureMap[v.ScheduleId] },
                        { "visitTime", v.VisitTime.ToString("o") }
                    }).ToList();

                if (visitParams.Any())
                {
                    // Здесь используем CREATE, т.к. каждое посещение уникально по времени
                    await tx.RunAsync(@"
                             UNWIND $batch AS props
                             MATCH (s:Student {id: props.studentId})
                             MATCH (l:Lecture {id: props.lectureId})
                             CREATE (s)-[:ATTENDED {visitTime: props.visitTime}]->(l)",
                        new { batch = visitParams });
                    _logger.LogDebug("Neo4j ATTENDED relationships created.");
                }
                else
                {
                    _logger.LogWarning("No valid visits found to create ATTENDED relationships.");
                }
            }
        });

        _logger.LogInformation("Neo4j Graph data saved.");

        _logger.LogInformation("Data saving finished successfully.");
        return new SaveResult(true, "Neo4j Graph data saved.");
    }
}
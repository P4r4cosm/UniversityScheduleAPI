using System.Text;
using Bogus;
using Elastic.Clients.Elasticsearch;
using LAB1_WEB_API.Services.Generators;
using MongoDB.Driver;
using Neo4j.Driver;
using StackExchange.Redis;

namespace LAB1_WEB_API.Services;

public class GeneratorService
{
    Faker faker;
    private ApplicationContext dbContext;
    private IMongoDatabase mongoDatabase;
    private ElasticsearchClient esClient;
    private IDatabase redis;
    private IDriver neo4j;
    
    public GeneratorService(ApplicationContext dbContext, ElasticsearchClient esClient, IMongoDatabase mongoDatabase,
        IDatabase redis, IDriver neo4j)
    {
        faker=new Faker("ru");
    }

    public string Generate()
    {
        var specialityGenerator = new SpecialtyGenerator(faker);
        var Specialties = new List<Speciality>();
        for (int i = 0; i < 10; i++)
        {
            var speciality= specialityGenerator.Generate();
            Specialties.Add(new Speciality(){Name = speciality});
        }
        
        var universityGenerator = new UniversityNameGenerator(faker);
        var Universities = new List<University>();
        for (int i = 0; i < 3; i++)
        {
            var university= universityGenerator.Generate();
            Universities.Add(new University(){Id=i+1,Name=university});
        }
        var instituteGenerator = new InstituteGenerator(faker, Universities);
        var Institutes = new List<Institute>();
        for (int i = 0; i < 30; i++)
        {
            var institute= instituteGenerator.Generate();
            Institutes.Add(new Institute{Id=i+1,Name=institute});
        }
        //генерируем департаменты
        var departmentgenerator = new DepartmentGenerator(faker,Institutes);
        var Departments = new List<Department>();
        for (int i = 0; i < 300; i++)
        {
            var department= departmentgenerator.Generate();
            Departments.Add(new Department(){Name=department});
        }
        
        
        //генерируем группы
        var groupGenerator = new GroupGenerator(faker, Departments);
        var Groups = new List<Group>();
        for (int i = 0; i < 1000; i++)
        {
            var group = groupGenerator.GenerateGroupData();
            Groups.Add(group);
        }
        //генерим студентов
        var studentGenerator = new StudentGenerator(faker, Groups);
        var Students = new List<Student>();
        for (int i = 0; i < 30000; i++)
        {
            var student= studentGenerator.Generate();
            Students.Add(new Student(){FullName=student});
        }
        
        var courcesGenerator = new CourseGenerator(faker, Departments,Specialties);
        var courses = new List<Course>();
        for (int i = 0; i < 100; i++)
        {
            var course = courcesGenerator.GenerateCourse();
            courses.Add(course);
        }
        
        var lectureGenerator = new LectureGenerator(faker);
        List<Lecture> Lectures = new List<Lecture>();
        foreach (var course in courses)
        {
            var lectureForCourse=lectureGenerator.GenerateLecturesForCourse(course);
            Lectures.AddRange(lectureForCourse);
        }
        
        var materialGenerator = new MaterialGenerator(faker);
        var materials = new List<Material>();
        foreach (var lecture in Lectures)
        {
            var materialsForLecture=materialGenerator.GenerateMaterialsForLecture(lecture);
            materials.AddRange(materialsForLecture);
        }
        var scheduleGenerator = new ScheduleGenerator(faker);
        var schedules = scheduleGenerator.Generate(Groups,Lectures,100);
        
        var visitGenerator = new VisitGenerator(faker);
        var visits = visitGenerator.Generate(Students,schedules);
        

        //генерируем материлы для эластика
        var materialElasticGenerator = new MaterialElasticGenerator(faker);
        var materialElasticsList = materialElasticGenerator.Generate(materials);
        
        
        
        
        return "Ok";
    }
}
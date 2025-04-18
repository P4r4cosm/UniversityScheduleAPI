using System.Text;
using Bogus;
using Elastic.Clients.Elasticsearch;
using LAB1_WEB_API.Infrastructure.Generators.Data;
using LAB1_WEB_API.Services.Generators;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Neo4j.Driver;
using StackExchange.Redis;

namespace LAB1_WEB_API.Services;

public class GeneratorService
{
    private readonly Faker _faker;


    private readonly ILogger<GeneratorService> _logger;

    public GeneratorService(
        ILogger<GeneratorService> logger)
    {
        _logger = logger;
        _faker = new Faker("ru");
    }

    public GeneratedData GenerateForPostgres(int SpecialtiesCount=300, int UniversityCount=3,
        int InstitutionCount=30, int DepartmentCount=300
    , int groupCount=100, int StudentCount=1000, int CourseCount=100)
    {
        _logger.LogInformation("Starting data generation (in memory)...");

        var specialityGenerator = new SpecialtyGenerator(_faker);
        // --- Генерация данных (без сохранения) ---
        _logger.LogDebug("Generating Specialties...");
        var Specialties = new List<Speciality>();
        for (int i = 0; i < SpecialtiesCount; i++)
        {
            var speciality = specialityGenerator.GenerateSpecialty();
            Specialties.Add(speciality);
        }


        _logger.LogDebug("Generating Universities...");
        var universityGenerator = new UniversityNameGenerator(_faker);
        var Universities = new List<University>();
        for (int i = 0; i < UniversityCount; i++)
        {
            var university = universityGenerator.Generate();
            Universities.Add(new University() { Name = university });
        }

        _logger.LogDebug("Generating Institutes...");
        var instituteGenerator = new InstituteGenerator(_faker, Universities);
        var Institutes = new List<Institute>();
        for (int i = 0; i < InstitutionCount; i++)
        {
            var institute = instituteGenerator.GenerateInstituteData();
            Institutes.Add(institute);
        }

        _logger.LogDebug("Generating Departments...");
        //генерируем департаменты
        var departmentgenerator = new DepartmentGenerator(_faker, Institutes);
        var Departments = new List<Department>();
        for (int i = 0; i < DepartmentCount; i++)
        {
            var department = departmentgenerator.GenerateDepartmentData();
            Departments.Add(department);
        }

        _logger.LogDebug("Generating Groups...");
        //генерируем группы
        var groupGenerator = new GroupGenerator(_faker, Departments);
        var Groups = new List<Group>();
        for (int i = 0; i < groupCount; i++)
        {
            var group = groupGenerator.GenerateGroupData();
            Groups.Add(group);
        }

        _logger.LogDebug("Generating Students...");
        //генерим студентов
        var studentGenerator = new StudentGenerator(_faker, Groups);
        var Students = new List<Student>();
        for (int i = 0; i < StudentCount; i++)
        {
            var student = studentGenerator.GenerateStudentData();
            Students.Add(student);
        }

        _logger.LogDebug("Generating Courses...");
        var courcesGenerator = new CourseGenerator(_faker, Departments, Specialties);
        var courses = new List<Course>();
        for (int i = 0; i < CourseCount; i++)
        {
            var course = courcesGenerator.GenerateCourse();
            courses.Add(course);
        }

        _logger.LogDebug("Generating Lectures...");
        var lectureGenerator = new LectureGenerator(_faker);
        List<Lecture> Lectures = new List<Lecture>();
        foreach (var course in courses)
        {
            var lectureForCourse = lectureGenerator.GenerateLecturesForCourse(course);
            Lectures.AddRange(lectureForCourse);
        }

        _logger.LogDebug("Generating Materials...");
        var materialGenerator = new MaterialGenerator(_faker);
        var materials = new List<Material>();
        foreach (var lecture in Lectures)
        {
            var materialsForLecture = materialGenerator.GenerateMaterialsForLecture(lecture);
            materials.AddRange(materialsForLecture);
        }

        _logger.LogDebug("Generating Schedules...");
        var scheduleGenerator = new ScheduleGenerator(_faker);
        var schedules = scheduleGenerator.Generate(Groups, Lectures, 100);

        _logger.LogDebug("Generating Visits...");
        var visitGenerator = new VisitGenerator(_faker);
        var visits = visitGenerator.Generate(Students, schedules);
        var generatedData = new GeneratedData()
        {
            Visits = visits,
            Institutes = Institutes,
            Departments = Departments,
            Groups = Groups,
            Students = Students,
            Lectures = Lectures,
            Materials = materials,
            Schedules = schedules,
            Courses = courses,
            Specialities = Specialties,
            Universities = Universities,
        };
        return generatedData;
    }

    public GeneratedData GenerateDataForElastic(GeneratedData generatedData)
    {
        var elasticGenerator = new MaterialElasticGenerator(_faker);
        generatedData.MaterialElastics = elasticGenerator.Generate(generatedData.Materials);
        return generatedData;
    }
}
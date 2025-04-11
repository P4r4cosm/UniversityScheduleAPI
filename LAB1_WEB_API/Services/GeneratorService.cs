using System.Text;
using Bogus;
using LAB1_WEB_API.Services.Generators;

namespace LAB1_WEB_API.Services;

public class GeneratorService
{
    Faker faker;

    public GeneratorService(ApplicationContext dbContext)
    {
        faker=new Faker("ru");
    }

    public string Generate()
    {
        var sb = new StringBuilder();
        //генерим студентов
        sb.AppendLine("=====================Students=======================");
        var studentGenerator = new StudentGenerator(faker);
        for (int i = 0; i < 10; i++)
        {
            var student= studentGenerator.Generate();
            sb.AppendLine(student);
        }

        //генерим специальности
        sb.AppendLine("=====================Specialties=======================");
        var specialityGenerator = new SpecialtyGenerator(faker);
        for (int i = 0; i < 10; i++)
        {
            var speciality= specialityGenerator.Generate();
            sb.AppendLine(speciality);
        }


        //генерируем универы
        sb.AppendLine("=====================University=======================");
        var universityGenerator = new UniversityNameGenerator(faker);
        var Universities = new List<University>();
        for (int i = 0; i < 10; i++)
        {
            var university= universityGenerator.Generate();
            Universities.Add(new University{Id=i+1,Name=university});
            sb.AppendLine(university);
        }
        var instituteGenerator = new InstituteGenerator(faker, Universities);
        //генерируем институты
        sb.AppendLine("=====================Institutes=======================");
        var Institutes = new List<Institute>();
        for (int i = 0; i < 10; i++)
        {
            var institute= instituteGenerator.Generate();
            Institutes.Add(new Institute{Id=i+1,Name=institute});
            sb.AppendLine(institute);
        }
        
        
        //генерируем департаменты
        var departmentgenerator = new DepartmentGenerator(faker,Institutes);
        sb.AppendLine("====================Departments=====================");
        var Departments = new List<Department>();
        for (int i = 0; i < 10; i++)
        {
            var department= departmentgenerator.Generate();
            Departments.Add(new Department{Id=i+1,Name=department});
            sb.AppendLine(department);
        }
        //генерируем группы
        var groupGenerator = new GroupGenerator(faker, Departments);
        sb.AppendLine("===============Groups============================");
        var Groups = new List<Group>();
        for (int i = 0; i < 10; i++)
        {
            var group = groupGenerator.Generate();
            sb.AppendLine(group);
        }
        
        
        return sb.ToString();
    }
}
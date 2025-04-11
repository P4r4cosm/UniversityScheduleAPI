using Bogus;
using Bogus.DataSets;
using LAB1_WEB_API.Interfaces.Generator;

namespace LAB1_WEB_API.Services.Generators;

public class StudentGenerator : IDataGenerator
{
    private  readonly Faker faker;

    public StudentGenerator(Faker Faker)
    {
        faker = Faker;
    }
    // Генерирует мужское отчество
    public string GenerateMalePatronymic(string fatherFirstName = null)
    {
        if (string.IsNullOrEmpty(fatherFirstName))
        {
            fatherFirstName = faker.Name.FirstName(Name.Gender.Male);
        }

        // Простое правило для суффикса (можно усложнить для большей точности)
        if (fatherFirstName.EndsWith("й") || fatherFirstName.EndsWith("ь"))
        {
            return fatherFirstName.Substring(0, fatherFirstName.Length - 1) + "евич";
        }
        else if (fatherFirstName.EndsWith("а") || fatherFirstName.EndsWith("я"))
        {
             // Имена типа Лука, Илья -> Ильич, Лукич. Никита -> Никитич
             // Для имен типа Фома -> Фомич
             // Это упрощенное правило может быть неточным для некоторых имен.
            if (fatherFirstName.EndsWith("а") && fatherFirstName.Length > 2 && "гкхчшщ".Contains(fatherFirstName[fatherFirstName.Length-2])) {
                 return fatherFirstName.Substring(0, fatherFirstName.Length - 1) + "ич"; // Лука -> Лукич? (проверить) - Кажется правило сложнее. Оставим упрощенно.
            }
             if (fatherFirstName == "Илья") return "Ильич";
             if (fatherFirstName == "Никита") return "Никитич";
              // Добавим базовые случаи для -а/-я
             if (fatherFirstName.EndsWith("а") || fatherFirstName.EndsWith("я"))
                return fatherFirstName.Substring(0, fatherFirstName.Length - 1) + "ич";

            return fatherFirstName + "ович"; // Общий случай
        }
        else
        {
            return fatherFirstName + "ович";
        }
    }

    // Генерирует женское отчество
    public string GenerateFemalePatronymic(string fatherFirstName = null)
    {
        if (string.IsNullOrEmpty(fatherFirstName))
        {
            fatherFirstName = faker.Name.FirstName(Name.Gender.Male);
        }

        // Простое правило для суффикса (можно усложнить)
         if (fatherFirstName.EndsWith("й") || fatherFirstName.EndsWith("ь"))
        {
            return fatherFirstName.Substring(0, fatherFirstName.Length - 1) + "евна";
        }
        else if (fatherFirstName.EndsWith("а") || fatherFirstName.EndsWith("я"))
        {
             // Илья -> Ильинична, Лука -> Лукинична, Никита -> Никитична
             // Фома -> Фоминична
             if (fatherFirstName == "Илья") return "Ильинична";
             if (fatherFirstName == "Никита") return "Никитична";
              // Добавим базовые случаи для -а/-я
             if (fatherFirstName.EndsWith("а") || fatherFirstName.EndsWith("я"))
                 return fatherFirstName.Substring(0, fatherFirstName.Length - 1) + "ична";

            return fatherFirstName + "овна"; // Общий случай
        }
        else
        {
            return fatherFirstName + "овна";
        }
    }

    public string Generate()
    {
        // 1. Случайно определяем пол для текущего человека
        var gender = faker.PickRandom<Name.Gender>(); // Выбираем Male или Female
        // 2. Генерируем случайное мужское имя (для имени отца)
        string fatherFirstName = faker.Name.FirstName(Name.Gender.Male);
        // 3. Генерируем части имени в зависимости от выбранного пола
        string firstName = faker.Name.FirstName(gender);
        string lastName = faker.Name.LastName(gender); // Bogus учтет окончание фамилии для женщин
        string patronymic;
        if (gender == Name.Gender.Male)
        {
            patronymic = GenerateMalePatronymic(fatherFirstName);
        }
        else // Name.Gender.Female
        {
            patronymic = GenerateFemalePatronymic(fatherFirstName);
        }
        // 4. Собираем полное ФИО
        string fullName = $"{firstName} {patronymic} {lastName}";
        return fullName;
    }
}
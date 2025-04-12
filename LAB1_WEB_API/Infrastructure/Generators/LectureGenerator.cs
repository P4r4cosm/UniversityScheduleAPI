using Bogus;

namespace LAB1_WEB_API.Services.Generators;

public class LectureGenerator
{
    private readonly Faker _faker;
    // Список курсов больше не нужен в конструкторе для основного метода,
    // но может понадобиться для вспомогательного метода генерации случайной лекции.
    private readonly List<Course> _courses;

    // --- Компоненты для названий Лекций (Более разнообразные) ---
    private static readonly string[] _introThemes = {
        "Введение в", "Основные понятия", "Обзор дисциплины:", "Цели и задачи курса:", "Исторический экскурс:"
    };

    private static readonly string[] _coreThemes = {
        "Теоретические основы", "Ключевые концепции", "Методология", "Анализ", "Моделирование",
        "Принципы", "Структура", "Классификация", "Стандартизация", "Проектирование", "Разработка",
        "Тестирование", "Оптимизация", "Применение", "Практикум по теме:"
    };

    private static readonly string[] _advancedThemes = {
        "Современные тенденции в", "Сравнительный анализ", "Перспективные разработки",
        "Критические аспекты", "Нестандартные подходы к", "Сложные задачи в",
        "Исследование", "Будущее развитие"
    };

    private static readonly string[] _conclusionThemes = {
        "Заключение по курсу:", "Подведение итогов:", "Обзор пройденного материала:", "Перспективы изучения", "Итоговый обзор:"
    };

    // Примеры подтем (очень общие, можно детализировать по областям)
    private static readonly string[] _subTopicExamples = {
        "алгоритмов", "структур данных", "баз данных", "сетевых протоколов", "операционных систем",
        "квантовой механики", "термодинамики", "электромагнетизма", "органической химии", "генетики",
        "микроэкономических моделей", "макроэкономической политики", "финансовых рынков", "маркетинговых стратегий",
        "исторических периодов", "философских школ", "социологических теорий", "правовых норм", "лингвистических аспектов"
    };


    // Конструктор может принимать список курсов для вспомогательных методов
    public LectureGenerator(Faker faker, IEnumerable<Course> courses = null)
    {
        _faker = faker ?? throw new ArgumentNullException(nameof(faker));
        // Сохраняем список, если он передан (для генерации случайной лекции)
        _courses = courses?.ToList();
    }

    // --- Основной метод: Генерация ПОСЛЕДОВАТЕЛЬНОСТИ лекций для КУРСА ---
    /// <summary>
    /// Генерирует список лекций для указанного курса.
    /// </summary>
    /// <param name="course">Курс, для которого генерируются лекции.</param>
    /// <param name="lectureCount">Количество лекций для генерации (рекомендуется 8-16).</param>
    /// <returns>Список сгенерированных объектов Lecture.</returns>
    public List<Lecture> GenerateLecturesForCourse(Course course) // Убрали параметр lectureCount
    {
        if (course == null) throw new ArgumentNullException(nameof(course));

        // *** Определяем случайное количество лекций ***
        int lectureCount = _faker.Random.Int(8, 16); // Генерируем от 8 до 16 лекций

        var lectures = new List<Lecture>();
        var usedSubtopics = new HashSet<string>();

        for (int i = 0; i < lectureCount; i++) // Используем сгенерированный lectureCount
        {
            int lectureNumber = i + 1;
            string lectureName;
            string theme;
            string subTopic = "";

            // Выбираем тип темы (логика та же)
            if (i == 0 && lectureCount > 1) { theme = _faker.PickRandom(_introThemes); }
            else if (i == lectureCount - 1 && lectureCount > 2) { theme = _faker.PickRandom(_conclusionThemes); }
            else {
                theme = _faker.PickRandom(_coreThemes.Concat(_advancedThemes));
                if (_faker.Random.Bool(0.6f) && _subTopicExamples.Any()) {
                    subTopic = _faker.PickRandom(_subTopicExamples.Except(usedSubtopics));
                    if (!string.IsNullOrEmpty(subTopic)) usedSubtopics.Add(subTopic);
                    else subTopic = _faker.PickRandom(_subTopicExamples);
                }
            }

            // Формируем название лекции (логика та же)
            string formattedLectureNumber = $"Лекция {lectureNumber}:";
            if (theme.EndsWith(":")) lectureName = $"{formattedLectureNumber} {theme.TrimEnd(':')} {subTopic}".Trim();
            else if (!string.IsNullOrEmpty(subTopic)) lectureName = $"{formattedLectureNumber} {theme} {subTopic}".Trim();
            else {
                lectureName = $"{formattedLectureNumber} {theme}".Trim();
                if (i > 0 && i < lectureCount - 1 && _faker.Random.Bool(0.3f)) lectureName += $" (на примере курса \"{course.Name}\")";
            }
            lectureName = lectureName.Replace("  ", " ");

            // Генерируем Requirments (логика та же)
            float requirementProbability = 0.1f + ((float)lectureNumber / lectureCount) * 0.4f;
            bool requirements = _faker.Random.Float() < requirementProbability;

            // Создаем объект Lecture (логика та же)
            var newLecture = new Lecture
            {
                Name = lectureName,
                Requirments = requirements,
                Course = course
            };
            lectures.Add(newLecture);
        }

        return lectures;
    }

     // --- Вспомогательный метод: Генерация ОДНОЙ случайной лекции для СЛУЧАЙНОГО курса ---
    /// <summary>
    /// Генерирует одну случайную лекцию для случайного курса из списка, переданного в конструктор.
    /// </summary>
    /// <returns>Объект Lecture или null, если список курсов не был предоставлен.</returns>
    public Lecture GenerateRandomLecture()
    {
        if (_courses == null || !_courses.Any())
        {
             // Можно выбросить исключение или вернуть null
             // throw new InvalidOperationException("Список курсов не был предоставлен в конструктор.");
             Console.Error.WriteLine("Предупреждение: Список курсов не был предоставлен, не могу сгенерировать случайную лекцию.");
             return null;
        }

        var selectedCourse = _faker.PickRandom(_courses);

        // Генерируем название (упрощенная логика без нумерации)
        string theme = _faker.PickRandom(_introThemes.Concat(_coreThemes).Concat(_advancedThemes));
        string subTopic = _faker.Random.Bool(0.5f) ? _faker.PickRandom(_subTopicExamples) : "";
        string lectureName;
         if (theme.EndsWith(":")) {
             lectureName = $"{theme.TrimEnd(':')} {subTopic}".Trim();
         } else if (!string.IsNullOrEmpty(subTopic)) {
             lectureName = $"{theme} {subTopic}".Trim();
         } else {
             lectureName = $"{theme} \"{selectedCourse.Name}\"".Trim();
         }
         lectureName = lectureName.Replace("  ", " ");

         // Генерируем Requirments со средней вероятностью
         bool requirements = _faker.Random.Bool(0.25f); // 25% шанс

         var newLecture = new Lecture
         {
             Name = lectureName,
             Requirments = requirements,
             CourceId = selectedCourse.Id
         };

         return newLecture;
    }
}

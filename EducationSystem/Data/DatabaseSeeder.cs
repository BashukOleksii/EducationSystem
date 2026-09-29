using EducationSystem.Data;
using SqlKata.Execution;
using System.Data;

namespace EducationSystem.Data
{
    public sealed class DatabaseSeeder
    {
        private readonly DatabaseConnectionFactory _connectionFactory;

        /*
         * Якщо хоча б в одній із цих таблиць є запис,
         * база вважається непорожньою і seed не виконується.
         */
        private static readonly string[] Tables =
        {
            "groups",
            "students",
            "teachers",
            "subjects",
            "teachers_subjects",
            "lessons",
            "grades",
            "attendance"
        };


        public DatabaseSeeder(
            DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory =
                connectionFactory;
        }


        /*
         * true  -> база була порожня і дані додані
         * false -> база вже містила дані, нічого не змінено
         */
        public async Task<bool> SeedIfEmptyAsync()
        {
            using QueryFactory db =
                _connectionFactory.CreateQueryFactory();

            /*
             * Відкриваємо одне з'єднання і використовуємо
             * одну транзакцію для всієї перевірки та вставки.
             */
            db.Connection.Open();

            using IDbTransaction transaction =
                db.Connection.BeginTransaction(
                    IsolationLevel.Serializable
                );

            try
            {
                bool isEmpty =
                    await IsDatabaseEmptyAsync(
                        db,
                        transaction
                    );


                if (!isEmpty)
                {
                    transaction.Commit();

                    return false;
                }


                await SeedAsync(
                    db,
                    transaction
                );


                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
        }


        private static async Task<bool> IsDatabaseEmptyAsync(
            QueryFactory db,
            IDbTransaction transaction)
        {
            foreach (string table in Tables)
            {
                IEnumerable<int> result =
                    await db
                        .Query(table)
                        .AsCount()
                        .GetAsync<int>(
                            transaction
                        );


                int count =
                    result.FirstOrDefault();


                /*
                 * Достатньо одного запису в будь-якій таблиці,
                 * щоб не виконувати seed.
                 */
                if (count > 0)
                {
                    return false;
                }
            }


            return true;
        }


        private static async Task SeedAsync(
            QueryFactory db,
            IDbTransaction transaction)
        {
            /*
             * =====================================================
             * 1. GROUPS
             * =====================================================
             */

            int groupPi231Id =
                await db
                    .Query("groups")
                    .InsertGetIdAsync<int>(
                        new
                        {
                            prefix = "ПІ",
                            number = 231,
                            is_active = true
                        },
                        transaction
                    );


            int groupPi232Id =
                await db
                    .Query("groups")
                    .InsertGetIdAsync<int>(
                        new
                        {
                            prefix = "ПІ",
                            number = 232,
                            is_active = true
                        },
                        transaction
                    );


            int groupKn221Id =
                await db
                    .Query("groups")
                    .InsertGetIdAsync<int>(
                        new
                        {
                            prefix = "КН",
                            number = 221,
                            is_active = true
                        },
                        transaction
                    );


            /*
             * =====================================================
             * 2. TEACHERS
             * =====================================================
             */

            int teacherBondarenkoId =
                await InsertTeacherAsync(
                    db,
                    transaction,
                    "Олена",
                    "Бондаренко",
                    "o.bondarenko@edu.local",
                    "Methodist"
                );


            int teacherMelnykId =
                await InsertTeacherAsync(
                    db,
                    transaction,
                    "Андрій",
                    "Мельник",
                    "a.melnyk@edu.local",
                    "II"
                );


            int teacherKovalId =
                await InsertTeacherAsync(
                    db,
                    transaction,
                    "Ірина",
                    "Коваль",
                    "i.koval@edu.local",
                    "I"
                );


            int teacherTkachenkoId =
                await InsertTeacherAsync(
                    db,
                    transaction,
                    "Сергій",
                    "Ткаченко",
                    "s.tkachenko@edu.local",
                    "III"
                );


            int teacherShevchenkoId =
                await InsertTeacherAsync(
                    db,
                    transaction,
                    "Марина",
                    "Шевченко",
                    "m.shevchenko@edu.local",
                    "Hight"
                );


            /*
             * =====================================================
             * 3. SUBJECTS
             * =====================================================
             */

            int databasesId =
                await InsertSubjectAsync(
                    db,
                    transaction,
                    "Бази даних",
                    90
                );


            int oopId =
                await InsertSubjectAsync(
                    db,
                    transaction,
                    "Об'єктно-орієнтоване програмування",
                    90
                );


            int webId =
                await InsertSubjectAsync(
                    db,
                    transaction,
                    "Веб-технології",
                    90
                );


            int algorithmsId =
                await InsertSubjectAsync(
                    db,
                    transaction,
                    "Алгоритми та структури даних",
                    90
                );


            int networksId =
                await InsertSubjectAsync(
                    db,
                    transaction,
                    "Комп'ютерні мережі",
                    90
                );


            /*
             * =====================================================
             * 4. STUDENTS
             * =====================================================
             */

            List<int> pi231Students =
                new List<int>
                {
                    await InsertStudentAsync(
                        db, transaction,
                        "Олексій", "Іваненко",
                        "oleksii.ivanenko@student.local",
                        groupPi231Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Марія", "Петренко",
                        "mariia.petrenko@student.local",
                        groupPi231Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Дмитро", "Ковальчук",
                        "dmytro.kovalchuk@student.local",
                        groupPi231Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Анна", "Шевчук",
                        "anna.shevchuk@student.local",
                        groupPi231Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Максим", "Бондар",
                        "maksym.bondar@student.local",
                        groupPi231Id
                    )
                };


            List<int> pi232Students =
                new List<int>
                {
                    await InsertStudentAsync(
                        db, transaction,
                        "Ігор", "Мороз",
                        "ihor.moroz@student.local",
                        groupPi232Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Софія", "Романюк",
                        "sofiia.romaniuk@student.local",
                        groupPi232Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Владислав", "Кравчук",
                        "vladyslav.kravchuk@student.local",
                        groupPi232Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Катерина", "Лисенко",
                        "kateryna.lysenko@student.local",
                        groupPi232Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Назар", "Поліщук",
                        "nazar.polishchuk@student.local",
                        groupPi232Id
                    )
                };


            List<int> kn221Students =
                new List<int>
                {
                    await InsertStudentAsync(
                        db, transaction,
                        "Артем", "Савчук",
                        "artem.savchuk@student.local",
                        groupKn221Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Юлія", "Олійник",
                        "yuliia.oliinyk@student.local",
                        groupKn221Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Роман", "Бойко",
                        "roman.boiko@student.local",
                        groupKn221Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Дарина", "Козак",
                        "daryna.kozak@student.local",
                        groupKn221Id
                    ),

                    await InsertStudentAsync(
                        db, transaction,
                        "Богдан", "Гнатюк",
                        "bohdan.hnatiuk@student.local",
                        groupKn221Id
                    )
                };


            /*
             * =====================================================
             * 5. TEACHERS_SUBJECTS
             * =====================================================
             */

            int bondarenkoDatabasesId =
                await InsertTeacherSubjectAsync(
                    db,
                    transaction,
                    teacherBondarenkoId,
                    databasesId,
                    null
                );


            int melnykOopId =
                await InsertTeacherSubjectAsync(
                    db,
                    transaction,
                    teacherMelnykId,
                    oopId,
                    null
                );


            int kovalWebId =
                await InsertTeacherSubjectAsync(
                    db,
                    transaction,
                    teacherKovalId,
                    webId,
                    1
                );


            int tkachenkoAlgorithmsId =
                await InsertTeacherSubjectAsync(
                    db,
                    transaction,
                    teacherTkachenkoId,
                    algorithmsId,
                    null
                );


            int shevchenkoNetworksId =
                await InsertTeacherSubjectAsync(
                    db,
                    transaction,
                    teacherShevchenkoId,
                    networksId,
                    2
                );


            /*
             * =====================================================
             * 6. LESSONS
             * =====================================================
             */

            DateTime today =
                DateTime.Today;


            int lessonDatabases1Id =
                await InsertLessonAsync(
                    db,
                    transaction,
                    "Вступ до реляційних баз даних",
                    bondarenkoDatabasesId,
                    groupPi231Id,
                    today.AddDays(-14)
                );


            int lessonDatabases2Id =
                await InsertLessonAsync(
                    db,
                    transaction,
                    "Індекси та оптимізація запитів",
                    bondarenkoDatabasesId,
                    groupPi231Id,
                    today.AddDays(-7)
                );


            int lessonOopId =
                await InsertLessonAsync(
                    db,
                    transaction,
                    "Інкапсуляція та наслідування",
                    melnykOopId,
                    groupPi232Id,
                    today.AddDays(-6)
                );


            int lessonWebId =
                await InsertLessonAsync(
                    db,
                    transaction,
                    "HTTP та REST",
                    kovalWebId,
                    groupPi231Id,
                    today.AddDays(-5)
                );


            int lessonAlgorithmsId =
                await InsertLessonAsync(
                    db,
                    transaction,
                    "Алгоритми сортування",
                    tkachenkoAlgorithmsId,
                    groupKn221Id,
                    today.AddDays(-4)
                );


            int lessonNetworksId =
                await InsertLessonAsync(
                    db,
                    transaction,
                    "Модель OSI",
                    shevchenkoNetworksId,
                    groupPi232Id,
                    today.AddDays(-3)
                );


            /*
             * =====================================================
             * 7. GRADES
             *
             * Не виставляємо оцінки всім студентам навмисно,
             * щоб у журналі були і порожні значення.
             * =====================================================
             */

            await InsertGradeAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[0],
                12
            );

            await InsertGradeAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[1],
                10
            );

            await InsertGradeAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[2],
                9
            );


            await InsertGradeAsync(
                db, transaction,
                lessonDatabases2Id,
                pi231Students[0],
                11
            );

            await InsertGradeAsync(
                db, transaction,
                lessonDatabases2Id,
                pi231Students[3],
                8
            );


            await InsertGradeAsync(
                db, transaction,
                lessonOopId,
                pi232Students[0],
                10
            );

            await InsertGradeAsync(
                db, transaction,
                lessonOopId,
                pi232Students[1],
                12
            );

            await InsertGradeAsync(
                db, transaction,
                lessonOopId,
                pi232Students[2],
                7
            );


            await InsertGradeAsync(
                db, transaction,
                lessonAlgorithmsId,
                kn221Students[0],
                11
            );

            await InsertGradeAsync(
                db, transaction,
                lessonAlgorithmsId,
                kn221Students[1],
                10
            );


            /*
             * =====================================================
             * 8. ATTENDANCE
             *
             * Так само частину записів залишаємо відсутніми,
             * щоб журнал демонстрував NULL-стан.
             * =====================================================
             */

            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[0],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[1],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[2],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases1Id,
                pi231Students[3],
                "missing"
            );


            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases2Id,
                pi231Students[0],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases2Id,
                pi231Students[3],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonDatabases2Id,
                pi231Students[4],
                "missing"
            );


            await InsertAttendanceAsync(
                db, transaction,
                lessonOopId,
                pi232Students[0],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonOopId,
                pi232Students[1],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonOopId,
                pi232Students[2],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonOopId,
                pi232Students[3],
                "missing"
            );


            await InsertAttendanceAsync(
                db, transaction,
                lessonAlgorithmsId,
                kn221Students[0],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonAlgorithmsId,
                kn221Students[1],
                "present"
            );

            await InsertAttendanceAsync(
                db, transaction,
                lessonAlgorithmsId,
                kn221Students[2],
                "missing"
            );


            /*
             * Щоб змінні не виглядали випадково невикористаними:
             * ці заняття залишені без оцінок/відвідування навмисно,
             * щоб можна було протестувати повністю порожній журнал.
             */
            _ = lessonWebId;
            _ = lessonNetworksId;
        }


        private static Task<int> InsertTeacherAsync(
            QueryFactory db,
            IDbTransaction transaction,
            string firstName,
            string lastName,
            string email,
            string category)
        {
            return db
                .Query("teachers")
                .InsertGetIdAsync<int>(
                    new
                    {
                        first_name = firstName,
                        last_name = lastName,
                        email,
                        category,
                        is_active = true
                    },
                    transaction
                );
        }


        private static Task<int> InsertSubjectAsync(
            QueryFactory db,
            IDbTransaction transaction,
            string name,
            int duration)
        {
            return db
                .Query("subjects")
                .InsertGetIdAsync<int>(
                    new
                    {
                        name,
                        duration,
                        is_active = true
                    },
                    transaction
                );
        }


        private static Task<int> InsertStudentAsync(
            QueryFactory db,
            IDbTransaction transaction,
            string firstName,
            string lastName,
            string email,
            int groupId)
        {
            return db
                .Query("students")
                .InsertGetIdAsync<int>(
                    new
                    {
                        first_name = firstName,
                        last_name = lastName,
                        email,
                        group_id = groupId
                    },
                    transaction
                );
        }


        private static Task<int> InsertTeacherSubjectAsync(
            QueryFactory db,
            IDbTransaction transaction,
            int teacherId,
            int subjectId,
            byte? subgroup)
        {
            return db
                .Query("teachers_subjects")
                .InsertGetIdAsync<int>(
                    new
                    {
                        teacher_id = teacherId,
                        subject_id = subjectId,
                        subgroup,
                        is_active = true
                    },
                    transaction
                );
        }


        private static Task<int> InsertLessonAsync(
            QueryFactory db,
            IDbTransaction transaction,
            string title,
            int teacherSubjectId,
            int groupId,
            DateTime date)
        {
            return db
                .Query("lessons")
                .InsertGetIdAsync<int>(
                    new
                    {
                        title,
                        /*
                         * У поточній схемі teacher_id
                         * посилається на teachers_subjects.id.
                         */
                        teacher_id = teacherSubjectId,
                        group_id = groupId,
                        date = date.Date
                    },
                    transaction
                );
        }


        private static async Task InsertGradeAsync(
            QueryFactory db,
            IDbTransaction transaction,
            int lessonId,
            int studentId,
            byte grade)
        {
            await db
                .Query("grades")
                .InsertAsync(
                    new
                    {
                        /*
                         * У поточній схемі assessment_id
                         * посилається на lessons.id.
                         */
                        assessment_id = lessonId,
                        student_id = studentId,
                        grade
                    },
                    transaction
                );
        }


        private static async Task InsertAttendanceAsync(
            QueryFactory db,
            IDbTransaction transaction,
            int lessonId,
            int studentId,
            string status)
        {
            await db
                .Query("attendance")
                .InsertAsync(
                    new
                    {
                        lesson_id = lessonId,
                        student_id = studentId,
                        status
                    },
                    transaction
                );
        }
    }
}

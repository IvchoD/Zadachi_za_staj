-- =====================================================================
--  Кръчма „Жадният Програмист“ – схема на базата данни (SQLite)
--
--  ВНИМАНИЕ: Приложението създава базата САМО от DatabaseHelper.cs.
--  Този скрипт е документация / за ръчно изпълнение при нужда.
--  Файлът на базата: krachma.db (до .exe файла).
-- =====================================================================

-- Клиентите на кръчмата
CREATE TABLE IF NOT EXISTS Programmers (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT,
    Name             TEXT NOT NULL,
    FavoriteLanguage TEXT NOT NULL
);

-- Тефтерът: една поръчка = един ред
CREATE TABLE IF NOT EXISTS Tabs (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    ProgrammerId INTEGER NOT NULL,
    ItemName     TEXT NOT NULL,
    Price        REAL NOT NULL,
    Date         TEXT NOT NULL,
    FOREIGN KEY (ProgrammerId) REFERENCES Programmers (Id) ON DELETE CASCADE
);

-- ---------------------------------------------------------------------
--  Примерни данни (зареждат се автоматично при първо стартиране,
--  само ако таблицата Programmers е празна)
-- ---------------------------------------------------------------------
INSERT INTO Programmers (Name, FavoriteLanguage) VALUES
    ('Иван Debugger',        'C#'),
    ('Мария Frontendova',    'JavaScript'),
    ('Георги Stackoverflow', 'Python'),
    ('Петър NullReference',  'Java'),
    ('Алекс ConsoleLog',     'TypeScript');

-- Забележка: в кода датите са относителни спрямо момента на стартиране.
INSERT INTO Tabs (ProgrammerId, ItemName, Price, Date) VALUES
    (1, 'Крафт бира',           6.50, datetime('now', '-240 minutes')),
    (1, 'Пържени картофи',      5.50, datetime('now', '-180 minutes')),
    (2, 'Бургер „404“',        11.90, datetime('now', '-200 minutes')),
    (2, 'Крафт бира',           6.50, datetime('now',  '-90 minutes')),
    (3, 'Шот „Null Pointer“',   7.00, datetime('now', '-300 minutes')),
    (3, 'Крафт бира',           6.50, datetime('now', '-250 minutes')),
    (3, 'Ракия „Legacy Code“', 13.00, datetime('now', '-210 minutes')),
    (3, 'Пържени картофи',      5.50, datetime('now', '-160 minutes')),
    (3, 'Бургер „404“',        11.90, datetime('now', '-100 minutes')),
    (3, 'Крафт бира',           6.50, datetime('now',  '-40 minutes')),
    (4, 'Пържени картофи',      5.50, datetime('now', '-130 minutes')),
    (4, 'Шот „Null Pointer“',   7.00, datetime('now',  '-60 minutes')),
    (5, 'Салата „Clean Code“',  8.90, datetime('now', '-150 minutes')),
    (5, 'Бургер „404“',        11.90, datetime('now',  '-45 minutes'));

-- ---------------------------------------------------------------------
--  Основни заявки, използвани от приложението (всички са параметризирани
--  в кода – никаква конкатенация на потребителски вход => без SQL Injection)
-- ---------------------------------------------------------------------

-- JOIN: всички поръчки с имена вместо ID-та
-- SELECT p.Name, p.FavoriteLanguage, t.ItemName, t.Price,
--        strftime('%d.%m.%Y %H:%M', t.Date)
-- FROM Tabs t
-- INNER JOIN Programmers p ON p.Id = t.ProgrammerId
-- ORDER BY t.Date DESC;

-- SUM: общ дълг на конкретен програмист
-- SELECT COALESCE(SUM(Price), 0) FROM Tabs WHERE ProgrammerId = @ProgrammerId;

-- Плащане на сметката (изтрива поръчките на човека)
-- DELETE FROM Tabs WHERE ProgrammerId = @ProgrammerId;

using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace ZhadniyatProgramist.Services
{
    /// <summary>
    /// Звукови ефекти на кръчмата.
    ///
    /// Правила:
    ///  1. Всеки звук първо се търси като .wav файл в
    ///     AppDomain.CurrentDomain.BaseDirectory + "Assets/Sounds/&lt;име&gt;.wav"
    ///     и ако съществува, се пуска асинхронно чрез SoundPlayer.
    ///  2. Ако файлът ЛИПСВА – НЕ се пускат системните звуци на Windows
    ///     (Hand/Exclamation звучат като грешка на самата система).
    ///     Просто не се случва нищо. Единствено при грешка (PlayError)
    ///     има кратък, тих Console.Beep – изпълнен на background нишка,
    ///     за да не блокира интерфейса.
    ///  3. Enabled = false реално спира ВСИЧКИ звуци, включително
    ///     фоновата атмосфера.
    ///  4. Липсващ или повреден файл НИКОГА не хвърля exception навън.
    ///
    /// Очаквани файлове в Assets/Sounds/ (по избор):
    ///   order.wav, payment.wav, error.wav, big_debt.wav, select.wav,
    ///   hover.wav, click.wav, ambience.wav (loop)
    /// </summary>
    public static class SoundService
    {
        /// <summary>Глобален превключвател. По подразбиране звукът е включен.</summary>
        public static bool Enabled { get; set; } = true;

        private static readonly string SoundsFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds");

        // Кеш на заредените SoundPlayer-и, за да не четем файла при всяко пускане.
        // Стойност null = вече знаем, че файлът липсва (не проверяваме диска пак).
        private static readonly Dictionary<string, SoundPlayer> PlayerCache =
            new Dictionary<string, SoundPlayer>(StringComparer.OrdinalIgnoreCase);

        private static SoundPlayer _ambiencePlayer; // фонов шум на кръчма (loop)

        // --- Събития в приложението -----------------------------------------

        public static void PlayOrderAdded() => Play("order.wav");
        public static void PlayPayment()    => Play("payment.wav");
        public static void PlayBigDebt()    => Play("big_debt.wav");
        public static void PlaySelect()     => Play("select.wav");
        public static void PlayHover()      => Play("hover.wav");
        public static void PlayClick()      => Play("click.wav");

        /// <summary>
        /// Звук при грешка/невалиден вход. Ако няма error.wav –
        /// кратък тих beep на background нишка (НЕ системният
        /// "exception" звук на Windows и без да блокира UI-а).
        /// </summary>
        public static void PlayError()
        {
            if (!Enabled) return;

            if (!Play("error.wav"))
            {
                try
                {
                    // Console.Beep е блокиращ, затова върви извън UI нишката.
                    Task.Run(() =>
                    {
                        try { Console.Beep(520, 90); }
                        catch { /* няма звукова карта / server core – тишина */ }
                    });
                }
                catch
                {
                    // Дори стартирането на задачата да се провали – тишина.
                }
            }
        }

        // --- Фонова атмосфера -------------------------------------------------

        /// <summary>
        /// Пуска лека кръчмарска атмосфера на loop, ако съществува
        /// Assets/Sounds/ambience.wav. Без файла просто не прави нищо.
        /// </summary>
        public static void StartAmbience()
        {
            if (!Enabled) return;

            try
            {
                string path = Path.Combine(SoundsFolder, "ambience.wav");
                if (!File.Exists(path)) return;

                if (_ambiencePlayer == null)
                {
                    _ambiencePlayer = new SoundPlayer(path);
                }
                _ambiencePlayer.PlayLooping();
            }
            catch
            {
                // Атмосферата е бонус – не пречим на приложението.
            }
        }

        /// <summary>Спира фоновата атмосфера (при изключен звук).</summary>
        public static void StopAmbience()
        {
            try
            {
                _ambiencePlayer?.Stop();
            }
            catch
            {
                // игнорираме
            }
        }

        // --- Вътрешно ---------------------------------------------------------

        /// <summary>
        /// Пуска .wav от Assets/Sounds асинхронно.
        /// Връща true, ако файлът съществува и е пуснат успешно.
        /// Липсващ файл = тишина (никакви системни звуци, никакъв exception).
        /// </summary>
        private static bool Play(string fileName)
        {
            if (!Enabled) return false;

            try
            {
                if (!PlayerCache.TryGetValue(fileName, out SoundPlayer player))
                {
                    string path = Path.Combine(SoundsFolder, fileName);
                    player = File.Exists(path) ? new SoundPlayer(path) : null;
                    PlayerCache[fileName] = player; // кешираме и липсата
                }

                if (player == null) return false;

                player.Play(); // асинхронно – не блокира интерфейса
                return true;
            }
            catch
            {
                // Повреден файл или проблем със звуковата карта:
                // маркираме звука като неизползваем и продължаваме тихо.
                PlayerCache[fileName] = null;
                return false;
            }
        }
    }
}

namespace PluginSupport
{
    public static class ParseHelper
    {
        /// <summary>
        /// Разбор символьной записи для получения координат точки
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Point ParsePoint(string line, Point defaultValue)
        {
            if (string.IsNullOrWhiteSpace(line)) return defaultValue;
            // Разбиваем по запятым и убираем пустые элементы
            string[] tokens = line.Trim('{', '}').Split([","], StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 2)
            {
                string valueX = tokens[0].Split('=').Last();
                string valueY = tokens[1].Split('=').Last();
                // Проверяем, что удалось успешно преобразовать обе координаты
                if (int.TryParse(valueX, out int x) && int.TryParse(valueY, out int y))
                    return new Point(x, y);
            }
            return defaultValue;
        }

        /// <summary>
        /// Разбор символьной записи для получения int значения
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static int ParseInteger(string line, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(line)) return defaultValue;
            var value = line;
            return int.TryParse(value, out int x) ? x : defaultValue;
        }

        /// <summary>
        /// Разбор символьной записи для получения bool значения
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool ParseBoolean(string line, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(line)) return defaultValue;
            var value = line;
            return bool.TryParse(value, out bool x) ? x : defaultValue;
        }
    }
}

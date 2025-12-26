# ActayWide Font Files

Эта папка должна содержать следующие файлы шрифтов:

- `ActayWide-Regular.woff2`
- `ActayWide-Regular.woff`
- `ActayWide-Bold.woff2`
- `ActayWide-Bold.woff`

## Как получить шрифты:

### Вариант 1: Скачать бесплатную версию
1. Перейдите на https://pixelsurplus.com/products/actay-free-geometric-grotesque-fonts
2. Скачайте бесплатную версию шрифта
3. Конвертируйте TTF файлы в WOFF и WOFF2 используя онлайн конвертер (например, https://cloudconvert.com/ttf-to-woff2)

### Вариант 2: Купить полную версию
1. Перейдите на https://www.myfonts.com/products/actay-wide-complete-package-437897/
2. После покупки вы получите файлы в различных форматах
3. Скопируйте нужные файлы в эту папку

### Вариант 3: Использовать конвертер TTF → WOFF/WOFF2
Если у вас есть TTF файлы:
1. Используйте онлайн конвертер: https://cloudconvert.com/ttf-to-woff2
2. Или используйте инструмент `woff2_compress` из пакета woff2-tools

## Структура файлов:
```
fonts/
├── ActayWide-Regular.woff2  (основной формат, самый маленький размер)
├── ActayWide-Regular.woff   (fallback для старых браузеров)
├── ActayWide-Bold.woff2     (жирный вариант)
└── ActayWide-Bold.woff      (жирный вариант, fallback)
```


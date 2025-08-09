FROM node AS build

# Рабочая директория в контейнере
WORKDIR /app

# Копируем package.json и package-lock.json/yarn.lock для кеширования установки зависимостей
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем весь исходный код
COPY . .

# Собираем проект (продакшен сборка Vite)
RUN npm run build

# --- Стадия запуска (продакшен) ---

# Минималистичный nginx для отдачи статики
FROM nginx:stable-alpine

# Копируем собранные файлы из предыдущего этапа в nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Копируем nginx конфиг (если нужен кастомный)
# COPY nginx.conf /etc/nginx/nginx.conf

# Порт, который будет слушать nginx
EXPOSE 80

# Запускаем nginx в форграунд режиме
CMD ["nginx", "-g", "daemon off;"]

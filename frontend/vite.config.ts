import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'
import * as path from 'path'

export default defineConfig({
  plugins: [react()],
  server: {
    fs: {
      allow: [
        // Разрешаем читать файлы из папки frontend (замени на свой полный путь)
        path.resolve(__dirname, '../frontend')
      ]
    }
  }
})

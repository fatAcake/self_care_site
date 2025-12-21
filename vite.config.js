import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// dev proxy: перенаправляет запросы /api к бэкенду (Kestrel)
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false
      }
    }
  }
})

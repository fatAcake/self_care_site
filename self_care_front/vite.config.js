import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'


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
    // Vite автоматически поддерживает SPA routing в dev режиме
  },
  // Для production сборки
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
    sourcemap: false,
    rollupOptions: {
      output: {
        manualChunks: undefined
      }
    }
  },
  // Fallback для SPA routing в preview режиме
  preview: {
    port: 4173,
    strictPort: true
  },
  // Плагин для обработки SPA routing в dev режиме
  // Vite уже поддерживает это по умолчанию, но явно указываем
  appType: 'spa'
})

import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// 개발 서버는 localhost:5173. Go WS 서버는 localhost:8080/ws (클라에서 직접 접속).
export default defineConfig({
  plugins: [react()],
  server: { host: 'localhost', port: 5173 },
})

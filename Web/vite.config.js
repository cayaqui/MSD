import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'https://ez-capst-dev-api-eastus.azurewebsites.net',
        changeOrigin: true,
        secure: false
      }
    }
  }
});
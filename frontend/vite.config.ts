import type { UserConfig, ConfigEnv } from "vite";
import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { resolve } from "path";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";
//https://github.com/element-plus/unplugin-element-plus/blob/HEAD/README.zh-CN.md
import ElementPlus from "unplugin-element-plus/vite";
export default defineConfig(({ command, mode }: ConfigEnv): UserConfig => {
  // const env = loadEnv(mode, process.cwd(), '')
  console.log(command, mode);
  return {
    plugins: [
      vue(),
      AutoImport({
        resolvers: [ElementPlusResolver()],
      }),
      Components({
        resolvers: [ElementPlusResolver()],
      }),
      ElementPlus({
        // useSource: true
      }),
    ],
    publicDir: "public",
    base: "./",
    server: {
      host: "0.0.0.0",
      port: 8112,
      open: false,
      strictPort: false,
      proxy: {
        "^/backend/": {
          target: "http://192.168.184.136:8080",
          changeOrigin: true,
          ws: true,
          rewrite: (path) => path.replace("/backend", ""),
          configure: (proxy, options) => {
            // 为特定代理实例启用调试日志
            proxy.on('error', (err, req, res) => {
              console.log('代理错误:', err);
            });
            proxy.on('proxyReq', (proxyReq, req, res) => {
              console.log('正在发送请求:', req.method, req.url);
            });
            proxy.on('proxyRes', (proxyRes, req, res) => {
              console.log('收到响应:', proxyRes.statusCode, req.url);
            });
          }
        },
        "^/backend-DS": {
          target: "http://192.168.184.136:8080",
          changeOrigin: true,
          ws: true,
          rewrite: (path) => path.replace("/backend-DS", ""),
          configure: (proxy, options) => {
            // 为特定代理实例启用调试日志
            proxy.on('error', (err, req, res) => {
              console.log('代理错误:', err);
            });
            proxy.on('proxyReq', (proxyReq, req, res) => {
              console.log('正在发送请求:', req.method, req.url);
            });
            proxy.on('proxyRes', (proxyRes, req, res) => {
              console.log('收到响应:', proxyRes.statusCode, req.url);
            });
          }
        },
        "^/backend-IS": {
          target: "http://192.168.184.136:8081",
          changeOrigin: true,
          ws: true,
          rewrite: (path) => path.replace("/backend-IS", ""),
          configure: (proxy, options) => {
            // 为特定代理实例启用调试日志
            proxy.on('error', (err, req, res) => {
              console.log('代理错误:', err);
            });
            proxy.on('proxyReq', (proxyReq, req, res) => {
              console.log('正在发送请求:', req.method, req.url);
            });
            proxy.on('proxyRes', (proxyRes, req, res) => {
              console.log('收到响应:', proxyRes.statusCode, req.url);
            });
          }
        },
      },
    },
    resolve: {
      alias: {
        "@": resolve(__dirname, "./src"),
        components: resolve(__dirname, "./src/components"),
        api: resolve(__dirname, "./src/api"),
      },
    },
    css: {
      // css预处理器
      preprocessorOptions: {
        scss: {
          // charset: false,
          additionalData: `@use "./src/assets/css/variable.scss" as *;`,
        },
      },
    },
    build: {
      outDir: "dist",
    },
  };
});

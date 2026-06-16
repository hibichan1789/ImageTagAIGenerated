import {defineConfig} from "vite";
import tailwindcss from "@tailwindcss/vite";


export default defineConfig({
    plugins: [tailwindcss()],
    build:{
        rollupOptions:{
            input:{
                home:"src/pages/home/index.html",
                index: "src/pages/login/login.html",
                register: "src/pages/register/register.html"
            }
        }
    }
})
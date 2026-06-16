import {defineConfig} from "vite";
import tailwindcss from "@tailwindcss/vite";


export default defineConfig({
    plugins: [tailwindcss()],
    build:{
        rollupOptions:{
            input:{
                home:"src/pages/home/home.html",
                index: "index.html",
                register: "src/pages/register/register.html"
            }
        }
    }
})
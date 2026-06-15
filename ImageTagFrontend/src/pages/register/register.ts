import { register } from "../../api/authApi";
import type { RegisterRequest } from "../../types/auth";

const form = document.getElementById("registerForm") as HTMLFormElement;
const emailInput = document.getElementById("email") as HTMLInputElement;
const passwordInput = document.getElementById("password") as HTMLInputElement;


form.addEventListener("submit", async (e) => {
    // submitされてもロードしないようにする
    e.preventDefault();

    const email = emailInput.value;
    const password = passwordInput.value;
    if (!email && !password) {
        window.alert("メールアドレスとパスワードを入力してください");
        return;
    }
    if (!email) {
        window.alert("メールアドレスを入力してください")
        return;
    }
    if (!password) {
        window.alert("パスワードを入力してください");
        return;
    }


    const registerRequest: RegisterRequest = {
        email,
        password
    };

    try {
        await register(registerRequest);

        window.alert("ユーザー登録が完了しました");
        window.location.href = "../login/login.html";
    }
    catch (err: any) {
        const message =
            err.response?.data?.message ||
            err.response?.data?.errors ||
            "登録に失敗しました";
        window.alert(message);
    }
})
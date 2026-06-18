import { login } from "./src/api/authApi";
import type { LoginRequest } from "./src/types/auth";


// HTML要素の取得
const form = document.getElementById("loginForm") as HTMLFormElement;
const emailInput = document.getElementById("email") as HTMLInputElement;
const passwordInput = document.getElementById("password") as HTMLInputElement;
const loginBtn = document.getElementById("loginBtn") as HTMLButtonElement;

// フォーム送信イベント
form.addEventListener("submit", async (e) => {
    e.preventDefault();



    const email = emailInput.value;
    const password = passwordInput.value;

    // 簡単なバリデーション
    if (!email && !password) {
        alert("メールアドレスとパスワードの入力をしてください");
        return;
    }
    if (!email) {
        alert("メールアドレスの入力をしてください");
        return;
    }
    if (!password) {
        alert("パスワードの入力をしてください");
        return;
    }

    // ログインボタンをクリックできなくする
    loginBtn.disabled = true;

    const loginRequest: LoginRequest = {
        email,
        password
    };

    // API呼び出し
    try {
        const response = await login(loginRequest);

        localStorage.setItem("token", response.token);

        window.alert("ログインに成功しました");
        location.href = "/src/pages/home/home.html";
    }
    catch (err: any) {
        const message =
            err.response?.data?.message ||
            err.response?.data?.errors ||
            "ログインに失敗しました";
        alert(message);
        
        // 失敗したらボタンを使えるようにする
        loginBtn.disabled = false;
    }
});
// ASP.NET.Core API とのやり取り

import {api} from "./axiosClient";
import type {
    RegisterRequest,
    RegisterResponse,
    LoginRequest,
    LoginResponse
}
from "../types/auth";

// axiosClient.tsからAPIクライアントを持ってくることでbaseURLやJWTを書く必要がなくなる
// 登録時
export async function register(data: RegisterRequest): Promise<RegisterResponse>{
    // 第一引数でbaseUrlからの相対パスを入れるとよい
    const response = await api.post<RegisterResponse>("auth/register", data);
    return response.data;
}

// ログイン時
export async function login(data: LoginRequest): Promise<LoginResponse>{
    const response = await api.post<LoginResponse>("auth/login", data);
    return response.data;
}
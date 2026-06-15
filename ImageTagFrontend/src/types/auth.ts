export interface RegisterRequest{
    email: string;
    password: string;
}

export interface RegisterResponse{
    userId: number;
    email: string;
    message: string;
}

export interface LoginRequest{
    email: string;
    password: string;
}

export interface LoginResponse{
    userId: number;
    email: string;
    token: string;
    expiredAt: string;
}
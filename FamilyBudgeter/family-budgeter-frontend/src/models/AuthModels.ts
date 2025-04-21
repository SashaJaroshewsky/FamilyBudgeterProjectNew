export interface UserRegistration {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
}

export interface UserLogin {
    email: string;
    password: string;
}

export interface AuthResponse {
    success: boolean;
    token?: string;
    refreshToken?: string;
    expiration?: Date;
    message?: string;
    user?: User;
}

export interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
}

export interface ChangePassword {
    currentPassword: string;
    newPassword: string;
}

export interface RefreshToken {
    token: string;
    refreshToken: string;
}
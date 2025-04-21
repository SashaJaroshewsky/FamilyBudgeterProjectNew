export enum FamilyRole {
    Administrator = 'Administrator',
    FullMember = 'FullMember',
    LimitedMember = 'LimitedMember'
}

export interface Family {
    id: number;
    name: string;
    joinCode?: string;
    members: FamilyMember[];
}

export interface FamilyMember {
    id: number;
    userId: number;
    userName: string;
    userEmail: string;
    role: FamilyRole;
}

export interface CreateFamily {
    name: string;
}

export interface UpdateFamily {
    name: string;
}

export interface JoinFamily {
    joinCode: string;
}

export interface FamilyMemberUpdate {
    userId: number;
    role: FamilyRole;
}
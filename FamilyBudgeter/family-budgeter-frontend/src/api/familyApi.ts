// src/api/familyApi.ts
import api from './api';
import { 
  Family, 
  CreateFamily, 
  UpdateFamily, 
  JoinFamily, 
  FamilyMember, 
  FamilyMemberUpdate 
} from '../models/FamilyModels';

export const familyApi = {
  /**
   * Отримання всіх сімей користувача
   */
  getUserFamilies: async (): Promise<Family[]> => {
    const response = await api.get<Family[]>('/family');
    return response.data;
  },

  /**
   * Отримання сім'ї за ідентифікатором
   */
  getFamilyById: async (id: number): Promise<Family> => {
    const response = await api.get<Family>(`/family/${id}`);
    return response.data;
  },

  /**
   * Створення нової сім'ї
   */
  createFamily: async (data: CreateFamily): Promise<Family> => {
    const response = await api.post<Family>('/family', data);
    return response.data;
  },

  /**
   * Оновлення сім'ї
   */
  updateFamily: async (id: number, data: UpdateFamily): Promise<Family> => {
    const response = await api.put<Family>(`/family/${id}`, data);
    return response.data;
  },

  /**
   * Видалення сім'ї
   */
  deleteFamily: async (id: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/family/${id}`);
    return response.data.success;
  },

  /**
   * Генерація нового коду приєднання
   */
  regenerateJoinCode: async (id: number): Promise<string> => {
    const response = await api.post<{ joinCode: string }>(`/family/${id}/regenerate-join-code`);
    return response.data.joinCode;
  },

  /**
   * Приєднання до сім'ї за кодом
   */
  joinFamily: async (data: JoinFamily): Promise<Family> => {
    const response = await api.post<Family>('/family/join', data);
    return response.data;
  },

  /**
   * Отримання членів сім'ї
   */
  getFamilyMembers: async (familyId: number): Promise<FamilyMember[]> => {
    const response = await api.get<FamilyMember[]>(`/family/${familyId}/members`);
    return response.data;
  },

  /**
   * Оновлення ролі члена сім'ї
   */
  updateMemberRole: async (familyId: number, memberUserId: number, data: FamilyMemberUpdate): Promise<FamilyMember> => {
    const response = await api.put<FamilyMember>(`/family/${familyId}/members/${memberUserId}`, data);
    return response.data;
  },

  /**
   * Видалення члена сім'ї
   */
  removeFamilyMember: async (familyId: number, memberUserId: number): Promise<boolean> => {
    const response = await api.delete<{ success: boolean }>(`/family/${familyId}/members/${memberUserId}`);
    return response.data.success;
  },

  /**
   * Вихід із сім'ї
   */
  leaveFamily: async (familyId: number): Promise<boolean> => {
    const response = await api.post<{ success: boolean }>(`/family/${familyId}/leave`);
    return response.data.success;
  }
};
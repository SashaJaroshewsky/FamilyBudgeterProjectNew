import { useState, useEffect } from 'react';
import { Category } from '../models/CategoryModels';
import { categoryApi } from '../api/categoryApi';

export const useCategories = (budgetId: number) => {
  const [categories, setCategories] = useState<Map<number, Category>>(new Map());
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const budgetCategories = await categoryApi.getBudgetCategories(budgetId);
        const categoryMap = new Map(budgetCategories.map(cat => [cat.id, cat]));
        setCategories(categoryMap);
      } catch (err) {
        setError('Помилка завантаження категорій');
      } finally {
        setLoading(false);
      }
    };

    fetchCategories();
  }, [budgetId]);

  return { categories, loading, error };
};
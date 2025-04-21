import React, { useState, useEffect } from 'react';
import { Container, Card, Row, Col, Form, Alert, Spinner } from 'react-bootstrap';
import { 
  PieChart, Pie, BarChart, Bar, XAxis, YAxis, CartesianGrid, 
  Tooltip, Legend, ResponsiveContainer, Cell 
} from 'recharts';
import DatePicker from 'react-datepicker';
import { budgetApi } from '../api/budgetApi';
import { transactionApi } from '../api/transactionApi';
import { Budget } from '../models/BudgetModels';
import { Transaction } from '../models/TransactionModels';
import AppHeader from '../components/common/AppHeader';
import 'react-datepicker/dist/react-datepicker.css';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884D8', '#82CA9D', '#FFC658'];

interface ChartData {
  name: string;
  value: number;
}

interface DailyData {
  date: string;
  amount: number;
}

const AnalysisPage: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Filters
  const [budgets, setBudgets] = useState<Budget[]>([]);
  const [selectedBudgetId, setSelectedBudgetId] = useState<string>('');
  const [dateRange, setDateRange] = useState({
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
    endDate: new Date()
  });

  // Data
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [categoryData, setCategoryData] = useState<ChartData[]>([]);
  const [userData, setUserData] = useState<ChartData[]>([]);
  const [dailyData, setDailyData] = useState<DailyData[]>([]);
  const [totalAmount, setTotalAmount] = useState(0);

  // Add new states for income data
  const [incomeCategoryData, setIncomeCategoryData] = useState<ChartData[]>([]);
  const [incomeUserData, setIncomeUserData] = useState<ChartData[]>([]);
  const [incomeDailyData, setIncomeDailyData] = useState<DailyData[]>([]);
  const [totalIncome, setTotalIncome] = useState(0);
  
  // Load initial data
  useEffect(() => {
    const fetchBudgets = async () => {
      try {
        const userBudgets = await budgetApi.getUserBudgets();
        setBudgets(userBudgets);
        if (userBudgets.length > 0) {
          setSelectedBudgetId(userBudgets[0].id.toString());
        }
      } catch (err) {
        setError('Помилка завантаження бюджетів');
      }
    };
    fetchBudgets();
  }, []);

  // Load transactions and process data
  useEffect(() => {
    const fetchData = async () => {
      if (!selectedBudgetId) return;

      try {
        setLoading(true);
        const budgetTransactions = await transactionApi.getBudgetTransactions(Number(selectedBudgetId));
        
        // Filter by date range and transaction type
        const filteredTransactions = budgetTransactions.filter(t => {
          const transDate = new Date(t.date);
          return transDate >= dateRange.startDate && transDate <= dateRange.endDate;
        });

        const expenses = filteredTransactions.filter(t => t.type === 'Expense');
        const incomes = filteredTransactions.filter(t => t.type === 'Income');

        setTransactions(filteredTransactions);

        // Process expense category data
        const expenseCategoryMap = new Map<string, number>();
        expenses.forEach(t => {
          const current = expenseCategoryMap.get(t.categoryName) || 0;
          expenseCategoryMap.set(t.categoryName, current + t.amount);
        });
        setCategoryData(Array.from(expenseCategoryMap.entries()).map(([name, value]) => ({ name, value })));

        // Process income category data
        const incomeCategoryMap = new Map<string, number>();
        incomes.forEach(t => {
          const current = incomeCategoryMap.get(t.categoryName) || 0;
          incomeCategoryMap.set(t.categoryName, current + t.amount);
        });
        setIncomeCategoryData(Array.from(incomeCategoryMap.entries()).map(([name, value]) => ({ name, value })));

        // Process expense user data
        const expenseUserMap = new Map<string, number>();
        expenses.forEach(t => {
          const current = expenseUserMap.get(t.createdByUserName) || 0;
          expenseUserMap.set(t.createdByUserName, current + t.amount);
        });
        setUserData(Array.from(expenseUserMap.entries()).map(([name, value]) => ({ name, value })));

        // Process income user data
        const incomeUserMap = new Map<string, number>();
        incomes.forEach(t => {
          const current = incomeUserMap.get(t.createdByUserName) || 0;
          incomeUserMap.set(t.createdByUserName, current + t.amount);
        });
        setIncomeUserData(Array.from(incomeUserMap.entries()).map(([name, value]) => ({ name, value })));

        // Process daily data for both
        const expenseDailyMap = new Map<string, number>();
        const incomeDailyMap = new Map<string, number>();
        
        expenses.forEach(t => {
          const date = new Date(t.date).toLocaleDateString('uk-UA');
          const current = expenseDailyMap.get(date) || 0;
          expenseDailyMap.set(date, current + t.amount);
        });

        incomes.forEach(t => {
          const date = new Date(t.date).toLocaleDateString('uk-UA');
          const current = incomeDailyMap.get(date) || 0;
          incomeDailyMap.set(date, current + t.amount);
        });

        setDailyData(Array.from(expenseDailyMap.entries())
          .map(([date, amount]) => ({ date, amount }))
          .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        );

        setIncomeDailyData(Array.from(incomeDailyMap.entries())
          .map(([date, amount]) => ({ date, amount }))
          .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        );

        // Calculate totals
        setTotalAmount(expenses.reduce((sum, t) => sum + t.amount, 0));
        setTotalIncome(incomes.reduce((sum, t) => sum + t.amount, 0));
        
        setError(null);
      } catch (err) {
        setError('Помилка завантаження даних');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [selectedBudgetId, dateRange]);

  const formatMoney = (amount: number): string => {
    return new Intl.NumberFormat('uk-UA', {
      style: 'currency',
      currency: 'UAH'
    }).format(amount);
  };

  if (loading) {
    return (
      <>
        <AppHeader />
        <Container className="py-4">
          <div className="text-center">
            <Spinner animation="border" />
          </div>
        </Container>
      </>
    );
  }

  return (
    <>
      <AppHeader />
      <Container className="py-4">
        <Card className="shadow-sm mb-4">
          <Card.Body>
            <h2 className="mb-4">Аналіз витрат</h2>

            {error && (
              <Alert variant="danger" dismissible onClose={() => setError(null)}>
                {error}
              </Alert>
            )}

            {/* Filters */}
            <Row className="g-3 mb-4">
              <Col md={4}>
                <Form.Group>
                  <Form.Label>Бюджет</Form.Label>
                  <Form.Select
                    value={selectedBudgetId}
                    onChange={(e) => setSelectedBudgetId(e.target.value)}
                  >
                    {budgets.map(budget => (
                      <option key={budget.id} value={budget.id}>
                        {budget.name}
                      </option>
                    ))}
                  </Form.Select>
                </Form.Group>
              </Col>
              <Col md={8}>
                <Form.Group>
                  <Form.Label>Період аналізу</Form.Label>
                  <div className="d-flex gap-2">
                    <DatePicker
                      selected={dateRange.startDate}
                      onChange={(date: Date) => setDateRange(prev => ({ ...prev, startDate: date }))}
                      className="form-control"
                      dateFormat="dd.MM.yyyy"
                    />
                    <DatePicker
                      selected={dateRange.endDate}
                      onChange={(date: Date) => setDateRange(prev => ({ ...prev, endDate: date }))}
                      className="form-control"
                      dateFormat="dd.MM.yyyy"
                    />
                  </div>
                </Form.Group>
              </Col>
            </Row>

            {/* Updated Summary */}
            <Card className="mb-4">
              <Card.Body>
                <Row>
                  <Col md={3}>
                    <h4>Загальна сума витрат</h4>
                    <p className="h2 text-danger">{formatMoney(totalAmount)}</p>
                  </Col>
                  <Col md={3}>
                    <h4>Загальна сума доходів</h4>
                    <p className="h2 text-success">{formatMoney(totalIncome)}</p>
                  </Col>
                  <Col md={3}>
                    <h4>Баланс</h4>
                    <p className={`h2 ${totalIncome - totalAmount >= 0 ? 'text-success' : 'text-danger'}`}>
                      {formatMoney(totalIncome - totalAmount)}
                    </p>
                  </Col>
                  <Col md={3}>
                    <h4>Кількість транзакцій</h4>
                    <p className="h2 text-info">{transactions.length}</p>
                  </Col>
                </Row>
              </Card.Body>
            </Card>

            {/* Charts */}
            <Row className="g-4">
              {/* Category expenses */}
              <Col md={6}>
                <Card>
                  <Card.Header>
                    <h5 className="mb-0">Витрати за категоріями</h5>
                  </Card.Header>
                  <Card.Body>
                    <div style={{ width: '100%', height: 300 }}>
                      <ResponsiveContainer>
                        <PieChart>
                          <Pie
                            data={categoryData}
                            dataKey="value"
                            nameKey="name"
                            cx="50%"
                            cy="50%"
                            outerRadius={80}
                            label
                          >
                            {categoryData.map((entry, index) => (
                              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip formatter={(value) => formatMoney(Number(value))} />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                  </Card.Body>
                </Card>
              </Col>

              {/* User expenses */}
              <Col md={6}>
                <Card>
                  <Card.Header>
                    <h5 className="mb-0">Витрати за користувачами</h5>
                  </Card.Header>
                  <Card.Body>
                    <div style={{ width: '100%', height: 300 }}>
                      <ResponsiveContainer>
                        <PieChart>
                          <Pie
                            data={userData}
                            dataKey="value"
                            nameKey="name"
                            cx="50%"
                            cy="50%"
                            outerRadius={80}
                            label
                          >
                            {userData.map((entry, index) => (
                              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip formatter={(value) => formatMoney(Number(value))} />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                  </Card.Body>
                </Card>
              </Col>

              {/* Income Category Chart */}
              <Col md={6}>
                <Card>
                  <Card.Header>
                    <h5 className="mb-0">Доходи за категоріями</h5>
                  </Card.Header>
                  <Card.Body>
                    <div style={{ width: '100%', height: 300 }}>
                      <ResponsiveContainer>
                        <PieChart>
                          <Pie
                            data={incomeCategoryData}
                            dataKey="value"
                            nameKey="name"
                            cx="50%"
                            cy="50%"
                            outerRadius={80}
                            label
                          >
                            {incomeCategoryData.map((entry, index) => (
                              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip formatter={(value) => formatMoney(Number(value))} />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                  </Card.Body>
                </Card>
              </Col>

              {/* Income User Chart */}
              <Col md={6}>
                <Card>
                  <Card.Header>
                    <h5 className="mb-0">Доходи за користувачами</h5>
                  </Card.Header>
                  <Card.Body>
                    <div style={{ width: '100%', height: 300 }}>
                      <ResponsiveContainer>
                        <PieChart>
                          <Pie
                            data={incomeUserData}
                            dataKey="value"
                            nameKey="name"
                            cx="50%"
                            cy="50%"
                            outerRadius={80}
                            label
                          >
                            {incomeUserData.map((entry, index) => (
                              <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                            ))}
                          </Pie>
                          <Tooltip formatter={(value) => formatMoney(Number(value))} />
                          <Legend />
                        </PieChart>
                      </ResponsiveContainer>
                    </div>
                  </Card.Body>
                </Card>
              </Col>

              {/* Combined Daily Chart */}
              <Col xs={12}>
                <Card>
                  <Card.Header>
                    <h5 className="mb-0">Доходи та витрати за днями</h5>
                  </Card.Header>
                  <Card.Body>
                    <div style={{ width: '100%', height: 400 }}>
                      <ResponsiveContainer>
                        <BarChart data={dailyData}>
                          <CartesianGrid strokeDasharray="3 3" />
                          <XAxis dataKey="date" />
                          <YAxis />
                          <Tooltip formatter={(value) => formatMoney(Number(value))} />
                          <Legend />
                          <Bar dataKey="amount" name="Витрати" fill="#ff4444" />
                          {incomeDailyData.map(income => (
                            <Bar 
                              key={income.date} 
                              dataKey={() => income.amount} 
                              name="Доходи" 
                              fill="#00C851" 
                            />
                          ))}
                        </BarChart>
                      </ResponsiveContainer>
                    </div>
                  </Card.Body>
                </Card>
              </Col>
            </Row>
          </Card.Body>
        </Card>
      </Container>
    </>
  );
};

export default AnalysisPage;
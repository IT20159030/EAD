import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import 'bootstrap/dist/css/bootstrap.min.css';
import useStore from './store/zustandStore';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import NotificationsPage from './pages/notification';
import About from './pages/About';
import ProductCategory from './pages/product/ProductCategory';
import MainLayout from './layouts/MainLayout';
import AuthLayout from './layouts/AuthLayout';

import './App.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 3,
      refetchOnWindowFocus: false,
      staleTime: 300000,
      onError: (error) => {
        console.error('Error fetching data:', error);
      },
    },
    mutations: {
      onError: (error) => {
        console.error('Error performing mutation:', error);
      },
    },
  },
});

function App() {
  const { user, setUser, clearUser } = useStore();

  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Routes>
          <Route path="/" element={<MainLayout />}>
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="products" element={<About />} />
            <Route path="categories" element={<ProductCategory />} />
            <Route path="orders" element={<About />} />
            <Route path="customers" element={<About />} />
            <Route path="vendors" element={<About />} />
            <Route path="staff" element={<About />} />
            <Route path="inventory" element={<About />} />
            <Route path="reports" element={<About />} />
            <Route path="/notifications" element={<NotificationsPage />} />
          </Route>
          <Route path="/" element={<AuthLayout />}>
            <Route path="login" element={<Login />} />
            <Route path="register" element={<Register />} />
          </Route>
        </Routes>
      </Router>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  );
}

export default App;

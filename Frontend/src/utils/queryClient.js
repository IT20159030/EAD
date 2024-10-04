import { QueryClient } from "@tanstack/react-query";

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 3,
      refetchOnWindowFocus: false,
      staleTime: 300000,
      onError: (error) => {
        console.error("Error fetching data:", error);
      },
    },
    mutations: {
      onError: (error) => {
        console.error("Error performing mutation:", error);
      },
    },
  },
});

export default queryClient;

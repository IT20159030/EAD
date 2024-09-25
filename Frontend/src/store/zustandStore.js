import { create } from "zustand";

// TODO: Change according to your needs
const useStore = create((set) => ({
  // states
  user: null,

  //   functions to update states
  setUser: (userDetails) => set({ user: userDetails }),
  clearUser: () => set({ user: null }),
}));

export default useStore;

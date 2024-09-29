import axiosInstance from "../utils/axiosInstanceWithAuth";
import { createContext, useContext, useEffect, useMemo, useState } from "react";

const AuthContext = createContext();

const AuthProvider = ({ children }) => {
  const [user, _setUser] = useState(() => {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  });

  const setUser = (newUser) => {
    _setUser(newUser);
  };

  useEffect(() => {
    if (user) {
      axiosInstance.defaults.headers.common["Authorization"] =
        "Bearer " + user.token;
      localStorage.setItem("user", JSON.stringify(user));
    } else {
      delete axiosInstance.defaults.headers.common["Authorization"];
      localStorage.removeItem("user");
    }
  }, [user]);

  const contextValue = useMemo(
    () => ({
      user: user,
      setUser: setUser,
    }),
    [user]
  );

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export const useAuth = () => {
  return useContext(AuthContext);
};

export default AuthProvider;

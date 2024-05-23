import { Route, Routes } from "react-router-dom";
import "./App.css";
import ViewPage from "./pages/ViewPage";
import ManagePage from "./pages/ManagePage";

function App() {
  return (
    <Routes>
      <Route path="/" element={<ViewPage />} />
      <Route path="/view" element={<ViewPage />} />
      <Route path="/manage" element={<ManagePage />} />
      <Route path="*" element={<ViewPage />}></Route>
    </Routes>
  );
}

export default App;

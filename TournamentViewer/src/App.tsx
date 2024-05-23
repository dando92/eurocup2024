import { Route, Routes } from "react-router-dom";
import "./App.css";
import ViewPage from "./pages/ViewPage";
import ManagePage from "./pages/ManagePage";
import Navbar from "./components/layout/Navbar";

function App() {
  return (
    <div>
      <Navbar/>
    <Routes>
      <Route path="/" element={<ViewPage />} />
      <Route path="/view" element={<ViewPage />} />
      <Route path="/manage" element={<ManagePage />} />
      <Route path="*" element={<ViewPage />}></Route>
    </Routes>
    </div>
  );
}

export default App;

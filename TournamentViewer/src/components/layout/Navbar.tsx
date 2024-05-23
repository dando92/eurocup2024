import Logo from '../../assets/icon.png';

export default function Navbar() {
  return <nav className="w-full h-16 bg-blu">
    <div className="lg:container lg:mx-auto mx-3 flex flex-row gap-10 items-center h-full">
        <img src={Logo} alt="logo" className="h-12 w-12 rounded-lg" />
    </div>
  </nav>;
}

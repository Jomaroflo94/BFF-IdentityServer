import { userData } from "../../contexts/User";

export default function Login() {
  const user = userData();
  const bffLogoutUrl = user?.data?.bffLogoutUrl;
  return (
    <a
      href={bffLogoutUrl ? bffLogoutUrl : "/bff/logout"}
      style={{ top: 0, position: "fixed", right: 0, margin: "10px" }}
    >
      Cerrar sesión
    </a>
  );
}

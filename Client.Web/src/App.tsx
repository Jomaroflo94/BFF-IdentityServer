import GetProgrammingLanguages from "./api/ProgrammingLanguageAPI";
import Login from "./components/login";
import Logout from "./components/logout";
import { userData } from "./contexts/User";
export default function App() {
  const user = userData();
  const programmingLanguages = GetProgrammingLanguages();

  return (
    <div>
      <div>
        {user.data ? (
          <>
            <Logout></Logout>
          </>
        ) : (
          <>
            <Login></Login>
          </>
        )}
      </div>
      <div>
        {programmingLanguages.isLoading ? (
          <div>Cargando...</div>
        ) : (
          <div>
            {programmingLanguages.data &&
              programmingLanguages.data.length > 0 && (
                <h1>Lenguajes de Programación</h1>
              )}
            {programmingLanguages.data?.map(
              (programmingLanguage: { id: string; name: string }) => (
                <div key={programmingLanguage.id}>
                  <pre>{JSON.stringify(programmingLanguage, null, 2)}</pre>
                </div>
              )
            )}
          </div>
        )}
      </div>
    </div>
  );
}

import axios from "axios";
import { useQuery } from "react-query";

const config = {
  headers: {
    "X-CSRF": "testCSRF", //TODO
  },
};

const fetchProgrammingLanguages = async () => {
  const response = await axios("/api/ProgrammingLanguage", config);
  return response.data;
};

const programmingLanguages = () => {
  const { isLoading, error, data } = useQuery(
    "programmingLanguages",
    fetchProgrammingLanguages,
    {
      retry: false,
    }
  );

  return { isLoading, error, data };
};

export default programmingLanguages;

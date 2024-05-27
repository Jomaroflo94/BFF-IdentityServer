import axios from "axios";
import { useEffect, useState } from "react";
import { useQuery } from "react-query";

interface User {
  name: string | undefined;
  given_name: string | undefined;
  family_name: string | undefined;
  sub: string;
  bffLogoutUrl: string;
}

const requestConfig = {
  headers: {
    "X-CSRF": "testCSRF",
  },
};

export const getUserInfo = async () => {
  const response = await axios.get("/bff/user", requestConfig);
  return response.data;
};

function getClaims() {
  const { data } = useQuery("claims", getUserInfo, {
    retry: false,
  });

  return data as [{ type: string; value: string }];
}

export function userData() {
  const claims = getClaims();
  const [data, setUser] = useState<User | null>(null);

  useEffect(() => {
    if (claims) {
      const name = claims.find((c) => c.type === "name")?.value ?? "";
      const given_name =
        claims.find((c) => c.type === "given_name")?.value ?? "";
      const family_name =
        claims.find((c) => c.type === "family_name")?.value ?? "";
      const sub = claims.find((c) => c.type === "sub")?.value ?? "";
      const bffLogoutUrl =
        claims.find((c) => c.type === "bff:logout_url")?.value ?? "";
      setUser({ name, given_name, family_name, sub, bffLogoutUrl });
    }
  }, [claims]);

  return { data };
}

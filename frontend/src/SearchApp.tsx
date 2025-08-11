import React, { useState } from "react";

type SearchResult = {
  id: string | number;
  title: string;
  description: string;
  email: string;
  name: string;
  tags: Tag[];
};

type Tag = {
  id: string;
  name: string;
};

export const SearchApp: React.FC = () => {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<SearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSearch = async () => {
    if (!query.trim()) return;
    setLoading(true);
    setError(null);

    try {
      const response = await fetch(`http://localhost:3000/books/search?term=${encodeURIComponent(query)}`);
      
      if (!response.ok) {
        throw new Error(`Ошибка сервера: ${response.statusText}`);
      }
      const data: SearchResult[] = await response.json();
      setResults(data);
    } catch (err: any) {
      setError(err.message || "Ошибка запроса");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: 600, margin: "2rem auto", fontFamily: "Arial, sans-serif" }}>
      <h1>Поиск</h1>
      <div style={{ marginBottom: 16 }}>
        <input
          type="text"
          placeholder="Введите запрос"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") handleSearch();
          }}
          style={{
            padding: "8px 12px",
            fontSize: 16,
            width: "70%",
            marginRight: 8,
            borderRadius: 4,
            border: "1px solid #ccc",
          }}
        />
        <button
          onClick={handleSearch}
          disabled={loading}
          style={{
            padding: "8px 16px",
            fontSize: 16,
            cursor: loading ? "not-allowed" : "pointer",
            borderRadius: 4,
            border: "none",
            backgroundColor: "#007bff",
            color: "white",
          }}
        >
          {loading ? "Идет поиск..." : "Найти"}
        </button>
      </div>

      {error && <div style={{ color: "red", marginBottom: 16 }}>{error}</div>}

      <div>
        {results.length === 0 && !loading && <p>Результатов нет</p>}

        {results.map((item) => (
          <div
            key={item.id}
            style={{
              border: "1px solid #ddd",
              borderRadius: 6,
              padding: 12,
              marginBottom: 12,
              boxShadow: "0 1px 3px rgba(0,0,0,0.1)",
            }}
          >
            <h3 style={{ margin: "0 0 8px 0" }}>{item.title}</h3>
            <p style={{ margin: 0 }}>{item.description}</p>
            {item.tags.map((tag => (<p key={tag.id}>{tag.name}</p>)))}
          </div>
        ))}
      </div>
    </div>
  );
};

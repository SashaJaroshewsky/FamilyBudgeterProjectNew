// src/components/common/Loader.tsx
import React from 'react';

const Loader: React.FC = () => {
  return (
    <div className="loader-container">
      <div className="loader"></div>
      <p>Завантаження...</p>
    </div>
  );
};

export default Loader;
import React from "react";

export const Counter = () => {
  const [count, setCount] = React.useState(0);
  return (
    <>
      <div>
        <b>{count}</b>
      </div>
      <button onClick={() => setCount((count) => count + 1)}>Increment!</button>
    </>
  );
};

import React, { useState, useRef, useEffect } from "react";
import "../css/ScrollDropdown.css";

export default function ScrollDropdown({
  items = [],
  value,
  onChange,
  onScrollBottom,
  placeholder,
}) {
  const [open, setOpen] = useState(false); // <-- track if dropdown is open
  const listRef = useRef();

  useEffect(() => {
    const handleScroll = () => {
      const el = listRef.current;
      if (!el) return;
      if (el.scrollTop + el.clientHeight >= el.scrollHeight - 5) {
        onScrollBottom?.();
      }
    };

    const el = listRef.current;
    if (el) el.addEventListener("scroll", handleScroll);
    return () => el?.removeEventListener("scroll", handleScroll);
  }, [onScrollBottom]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (listRef.current && !listRef.current.parentNode.contains(e.target)) {
        setOpen(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <div className="scroll-dropdown">
      <div className="selected" onClick={() => setOpen((prev) => !prev)}>
        {value || placeholder}
      </div>
      {open && (
        <div className="options" ref={listRef}>
          {items.map((item, idx) => (
            <div
              key={idx}
              className="option"
              onClick={() => {
                onChange(item);
                setOpen(false); // close after selecting
              }}
            >
              {item}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

import React from 'react';
import './App.css';

function App() {
  return (
    <div className="App">
      {/* Верхний блок прогресса */}
      <div className="progress-section">
        <div className="progress-header">картинка/прогресс</div>
        <div className="progress-subheader">сегодняшний прогресс</div>
        <div className="progress-value">1/3</div>
      </div>

      {/* Блок добавления привычки */}
      <div className="add-habit-section">
        <span>Добавить привычку</span>
        <button className="add-button">+</button>
      </div>

      {/* Список привычек */}
      <div className="habits-list">
        <div className="habit-item">
          <span>полезная привычка #1</span>
          <div className="checkbox empty"></div>
        </div>
        <div className="habit-item">
          <span>полезная привычка #1</span>
          <div className="checkbox checked"></div>
        </div>
        <div className="habit-item">
          <span>полезная привычка #1</span>
          <div className="checkbox checked with-badge">
            <span className="badge">1/2</span>
          </div>
        </div>
      </div>

      {/* Нижняя навигация */}
      <div className="bottom-nav">
        <button className="nav-button">лого/главная страница</button>
        <button className="nav-button">материалы</button>
        <button className="nav-button">приложения</button>
      </div>
    </div>
  );
}

export default App;
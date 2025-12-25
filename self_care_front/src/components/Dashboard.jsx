import { useState, useEffect } from 'react';
import { getUserHabits, getProgress, toggleHabit, deleteUserHabit, addHabitToUser } from '../api/habitApi';
import { loadToken } from '../api/authApi';
import AddHabitModal from './AddHabitModal';
import Header from './Header';
import './style/Dashboard.css';
import mark from '../assets/mark.svg';
import arrow from '../assets/arrow.svg';
import trash from '../assets/trash.svg';

export default function Dashboard() {
    const [selectedDate, setSelectedDate] = useState(new Date());
    const [habits, setHabits] = useState([]);
    const [progress, setProgress] = useState({ completed: 0, total: 0 });
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [loading, setLoading] = useState(true);
    const [swipedHabitId, setSwipedHabitId] = useState(null);
    const [swipedLeftHabitId, setSwipedLeftHabitId] = useState(null); 
    const [dragStartX, setDragStartX] = useState(null);

    useEffect(() => {
        loadToken();
    }, []);

    const weekDays = ['ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ', 'ВС'];

    const getDayOfWeek = (date) => {
        const day = date.getDay();
        return day === 0 ? 6 : day - 1;
    };

    const selectedDayIndex = getDayOfWeek(selectedDate);

    const getDateForDay = (dayIndex) => {
        const today = new Date();
        const currentDay = getDayOfWeek(today);
        const diff = dayIndex - currentDay;
        const targetDate = new Date(today);
        targetDate.setDate(today.getDate() + diff);
        return targetDate;
    };

    const loadData = async () => {
        try {
            setLoading(true);
            const [habitsData, progressData] = await Promise.all([
                getUserHabits(selectedDate),
                getProgress(selectedDate)
            ]);
            setHabits(habitsData);
            setProgress(progressData);
        } catch (error) {
            console.error('Error loading data:', error);
            alert('Ошибка загрузки данных');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, [selectedDate]);

    const handleDayClick = (dayIndex) => {
        const date = getDateForDay(dayIndex);
        setSelectedDate(date);
    };

    const handleToggleHabit = async (userHabitId) => {
        try {
            await toggleHabit(userHabitId);
            await loadData();
        } catch (error) {
            console.error('Error toggling habit:', error);
            alert('Ошибка при отметке привычки');
        }
    };

    const handleHabitAdded = () => {
        setIsModalOpen(false);
        loadData();
    };

    const handleDeleteHabit = async (userHabitId) => {
        try {
            await deleteUserHabit(userHabitId);
            setSwipedHabitId(null);
            await loadData();
        } catch (error) {
            console.error('Error deleting habit:', error);
            alert('Ошибка удаления привычки');
        }
    };

    const handleMoveHabitNextDay = async (habit) => {
        try {
            const nextDate = new Date(selectedDate);
            nextDate.setDate(nextDate.getDate() + 1);
   
            await addHabitToUser(habit.habitId, nextDate);
            setSwipedLeftHabitId(null);
            await loadData();
        } catch (error) {
            console.error('Error moving habit:', error);
            const msg = error?.response?.data?.error || 'Ошибка при переносе привычки';
            alert(msg);
        }
    };

    const handleTouchStart = (e, habitId) => {
        const touch = e.touches[0];
        e.currentTarget.dataset.startX = touch.clientX;
        setDragStartX(touch.clientX);
    };

    const handleTouchMove = (e, habitId) => {
        const touch = e.touches[0];
        const startX = parseFloat(e.currentTarget.dataset.startX || '0');
        const diff = touch.clientX - startX;

        if (diff > 40) {
            setSwipedHabitId(habitId);
            setSwipedLeftHabitId(null);
        } else if (diff < -40) {
            setSwipedLeftHabitId(habitId);
            setSwipedHabitId(null);
        } else if (Math.abs(diff) < 10) {
            if (swipedHabitId === habitId) setSwipedHabitId(null);
            if (swipedLeftHabitId === habitId) setSwipedLeftHabitId(null);
        }
    };

    const handleMouseDown = (e, habitId) => {
        setDragStartX(e.clientX);
        e.currentTarget.dataset.mouseDown = 'true';
        e.currentTarget.dataset.startX = e.clientX.toString();
    };

    const handleMouseMove = (e, habitId) => {
        if (e.currentTarget.dataset.mouseDown !== 'true') return;
        const startX = parseFloat(e.currentTarget.dataset.startX || '0');
        const diff = e.clientX - startX;

        if (diff > 40) {
            setSwipedHabitId(habitId);
            setSwipedLeftHabitId(null);
        } else if (diff < -40) {
            setSwipedLeftHabitId(habitId);
            setSwipedHabitId(null);
        } else if (Math.abs(diff) < 10) {
            if (swipedHabitId === habitId) setSwipedHabitId(null);
            if (swipedLeftHabitId === habitId) setSwipedLeftHabitId(null);
        }
    };

    const handleMouseUpLeave = (e) => {
        e.currentTarget.dataset.mouseDown = 'false';
        setDragStartX(null);
    };

    const progressPercentage = progress.total > 0
        ? (progress.completed / progress.total) * 100
        : 0;

    return (
        <div className="dashboard">
            <div className="dashboard-content">
                <div className="progress-card">
                    <div className="progress-circle-container">
                        <svg className="progress-circle" viewBox="0 0 100 100">
                            <circle className="progress-circle-bg" cx="50" cy="50" r="45" />
                            <circle
                                className="progress-circle-fill"
                                cx="50"
                                cy="50"
                                r="45"
                                strokeDasharray={`${2 * Math.PI * 45}`}
                                strokeDashoffset={`${2 * Math.PI * 45 * (1 - progressPercentage / 100)}`}
                            />
                        </svg>
                        <div className="progress-text">{progress.completed}/{progress.total}</div>
                    </div>
                </div>

                <div className="week-days">
                    {weekDays.map((day, index) => (
                        <button
                            key={index}
                            className={`day-button ${selectedDayIndex === index ? 'active' : ''}`}
                            onClick={() => handleDayClick(index)}
                        >
                            {day}
                        </button>
                    ))}
                </div>

                <button className="add-button" onClick={() => setIsModalOpen(true)}>
                    <span>Добавить</span>
                    <span className="add-icon">+</span>
                </button>

                <div className="habits-list">
                    {loading ? (
                        <div className="loading">Загрузка...</div>
                    ) : habits.length === 0 ? (
                        <div className="empty-state">Нет привычек на этот день</div>
                    ) : (
                        habits.map((habit, index) => (
                            <div
                                key={habit.usHabitId}
                                className={`habit-row ${swipedHabitId === habit.usHabitId ? 'swiped-right' : ''} ${swipedLeftHabitId === habit.usHabitId ? 'swiped-left' : ''}`}
                                onTouchStart={(e) => handleTouchStart(e, habit.usHabitId)}
                                onTouchMove={(e) => handleTouchMove(e, habit.usHabitId)}
                                onMouseDown={(e) => handleMouseDown(e, habit.usHabitId)}
                                onMouseMove={(e) => handleMouseMove(e, habit.usHabitId)}
                                onMouseUp={handleMouseUpLeave}
                                onMouseLeave={handleMouseUpLeave}
                            >
                                <button
                                    className="habit-delete-button"
                                    onClick={() => handleDeleteHabit(habit.usHabitId)}
                                >
                                    <img src={trash} alt="Удалить" />
                                </button>

                                <button
                                    className="habit-move-button"
                                    onClick={() => handleMoveHabitNextDay(habit)}
                                    aria-label="Перенести на следующий день"
                                >
                                    <img src={arrow} alt="" className="habit-move-icon" aria-hidden="true" />
                                </button>

                                <div className={`habit-item ${habit.isMarked ? 'marked' : ''}`}>
                                    <span className="habit-name">{habit.habitName}</span>

                                    <div className="habit-action">
                                        {swipedLeftHabitId === habit.usHabitId ? (
                                            <div className="habit-move-column" onClick={() => handleMoveHabitNextDay(habit)} role="button" tabIndex={0}>
                                                <span>Перенести</span>
                                                <span>на следующий день</span>
                                            </div>
                                        ) : (
                                            <button
                                                className={`habit-toggle ${habit.isMarked ? 'checked' : ''}`}
                                                onClick={() => handleToggleHabit(habit.usHabitId)}
                                                aria-pressed={habit.isMarked}
                                                aria-label={habit.isMarked ? 'Отмечено' : 'Не отмечено'}
                                            >
                                                {habit.isMarked && (
                                                    <img src={mark} alt="" className="habit-mark" aria-hidden="true" />
                                                )}
                                                <span className="sr-only">{habit.isMarked ? 'Отмечено' : 'Не отмечено'}</span>
                                            </button>
                                        )}
                                    </div>
                                </div>
                            </div>
                        ))
                    )}
                </div>

                {isModalOpen && (
                    <AddHabitModal
                        selectedDate={selectedDate}
                        onClose={() => setIsModalOpen(false)}
                        onHabitAdded={handleHabitAdded}
                    />
                )}
            </div>

            <Header />
        </div>
    );
}


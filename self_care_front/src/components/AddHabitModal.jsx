import { useState, useEffect } from 'react';
import { getAllHabits, addHabitToUser, createHabit } from '../api/habitApi';
import './style/AddHabitModal.css';

export default function AddHabitModal({ selectedDate, onClose, onHabitAdded }) {
    const [habits, setHabits] = useState([]);
    const [executeDate, setExecuteDate] = useState(
        selectedDate.toISOString().split('T')[0]
    );
    const [isCreatingNew, setIsCreatingNew] = useState(false);
    const [newHabitName, setNewHabitName] = useState('');
    const [newHabitDescription, setNewHabitDescription] = useState('');
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        loadHabits();
    }, []);

    const loadHabits = async () => {
        try {
            setLoading(true);
            const data = await getAllHabits();
            setHabits(data);
        } catch (error) {
            console.error('Error loading habits:', error);
            alert('Ошибка загрузки привычек');
        } finally {
            setLoading(false);
        }
    };

    const handleAddHabit = async (habitId) => {
        try {
            setSubmitting(true);
            await addHabitToUser(habitId, new Date(executeDate));
            onHabitAdded();
        } catch (error) {
            console.error('Error adding habit:', error);
            alert(error.response?.data?.error || 'Ошибка добавления привычки');
        } finally {
            setSubmitting(false);
        }
    };

    const handleCreateAndAdd = async (e) => {
        e.preventDefault();
        
        if (!newHabitName.trim()) {
            alert('Введите название привычки');
            return;
        }
            
            try {
                setSubmitting(true);
                const newHabit = await createHabit(newHabitName, newHabitDescription);
                await addHabitToUser(newHabit.habitsId, new Date(executeDate));
                onHabitAdded();
            } catch (error) {
                console.error('Error creating habit:', error);
                alert('Ошибка создания привычки');
            } finally {
                setSubmitting(false);
            }
    };

    if (isCreatingNew) {
    return (
            <div className="add-habit-fullscreen">
                <div className="add-habit-header">
                    <button className="back-button" onClick={() => setIsCreatingNew(false)}>
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M15 18L9 12L15 6" stroke="white" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                        </button>
                    </div>
                <div className="add-habit-content">
                    <h2 className="create-habit-title">Добавить Свою Привычку</h2>
                    <form onSubmit={handleCreateAndAdd} className="create-habit-form">
                            <div className="form-group">
                                <input
                                    type="text"
                                    value={newHabitName}
                                    onChange={(e) => setNewHabitName(e.target.value)}
                                    required
                                placeholder="Название привычки"
                                className="habit-input"
                                />
                            </div>
                            <div className="form-group">
                                <textarea
                                    value={newHabitDescription}
                                    onChange={(e) => setNewHabitDescription(e.target.value)}
                                placeholder="Описание (необязательно)"
                                rows="4"
                                className="habit-textarea"
                                />
                        </div>
                            <div className="form-group">
                            <label className="date-label">Дата выполнения</label>
                        <input
                            type="date"
                            value={executeDate}
                            onChange={(e) => setExecuteDate(e.target.value)}
                            required
                                className="habit-date-input"
                        />
                    </div>
                        <button
                            type="submit"
                            className="submit-create-button"
                            disabled={submitting}
                        >
                            {submitting ? 'Сохранение...' : 'Добавить'}
                        </button>
                    </form>
                </div>
            </div>
        );
    }

    return (
        <div className="add-habit-fullscreen">
            <div className="add-habit-header">
                <button className="back-button" onClick={onClose}>
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M15 18L9 12L15 6" stroke="white" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                </button>
            </div>
            <div className="add-habit-content">
                <button className="add-habit-main-button" onClick={() => setIsCreatingNew(true)}>
                    Добавить привычку
                </button>

                <div className="habits-list-container">
                    {loading ? (
                        <div className="loading">Загрузка...</div>
                    ) : habits.length === 0 ? (
                        <div className="empty-state">Нет доступных привычек</div>
                    ) : (
                        <>
                            {habits.map((habit) => (
                                <div key={habit.habitsId} className="habit-card">
                                    <span className="habit-card-name">{habit.name}</span>
                                    <button
                                        className="habit-add-button"
                                        onClick={() => handleAddHabit(habit.habitsId)}
                                        disabled={submitting}
                                    >
                                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                            <path d="M12 6V18M6 12H18" stroke="white" strokeWidth="3" strokeLinecap="round"/>
                                        </svg>
                                    </button>
                                </div>
                            ))}
                            <div className="habit-card create-habit-card" onClick={() => setIsCreatingNew(true)}>
                                <span className="habit-card-name">Добавить Свою Привычку</span>
                                <button className="habit-add-button">
                                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M12 6V18M6 12H18" stroke="white" strokeWidth="3" strokeLinecap="round"/>
                                    </svg>
                                </button>
                            </div>
                        </>
                    )}
                    </div>
            </div>
        </div>
    );
}


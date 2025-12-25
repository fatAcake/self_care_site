import { useState, useEffect } from 'react';
import { getAllHabits, addHabitToUser, createHabit } from '../api/habitApi';
import './style/AddHabitModal.css';

export default function AddHabitModal({ selectedDate, onClose, onHabitAdded }) {
    const [habits, setHabits] = useState([]);
    const [selectedHabitId, setSelectedHabitId] = useState(null);
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

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (isCreatingNew) {
            
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
        } else {
            
            if (!selectedHabitId) {
                alert('Выберите привычку');
                return;
            }

            try {
                setSubmitting(true);
                await addHabitToUser(selectedHabitId, new Date(executeDate));
                onHabitAdded();
            } catch (error) {
                console.error('Error adding habit:', error);
                alert(error.response?.data?.error || 'Ошибка добавления привычки');
            } finally {
                setSubmitting(false);
            }
        }
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                <button className="modal-close" onClick={onClose}>×</button>
                
                <h2 className="modal-title">Добавить привычку</h2>

                <form onSubmit={handleSubmit}>
                   
                    <div className="modal-tabs">
                        <button
                            type="button"
                            className={`tab-button ${!isCreatingNew ? 'active' : ''}`}
                            onClick={() => setIsCreatingNew(false)}
                        >
                            Выбрать существующую
                        </button>
                        <button
                            type="button"
                            className={`tab-button ${isCreatingNew ? 'active' : ''}`}
                            onClick={() => setIsCreatingNew(true)}
                        >
                            Создать новую
                        </button>
                    </div>

                    {isCreatingNew ? (
                        
                        <div className="form-section">
                            <div className="form-group">
                                <label>Название привычки</label>
                                <input
                                    type="text"
                                    value={newHabitName}
                                    onChange={(e) => setNewHabitName(e.target.value)}
                                    required
                                    placeholder="Например: Пить воду"
                                />
                            </div>
                            <div className="form-group">
                                <label>Описание (необязательно)</label>
                                <textarea
                                    value={newHabitDescription}
                                    onChange={(e) => setNewHabitDescription(e.target.value)}
                                    placeholder="Описание привычки"
                                    rows="3"
                                />
                            </div>
                        </div>
                    ) : (
                        
                        <div className="form-section">
                            <div className="form-group">
                                <label>Выберите привычку</label>
                                {loading ? (
                                    <div className="loading">Загрузка...</div>
                                ) : habits.length === 0 ? (
                                    <div className="empty-state">Нет доступных привычек</div>
                                ) : (
                                    <div className="habits-select">
                                        {habits.map((habit) => (
                                            <button
                                                key={habit.habitsId}
                                                type="button"
                                                className={`habit-option ${selectedHabitId === habit.habitsId ? 'selected' : ''}`}
                                                onClick={() => setSelectedHabitId(habit.habitsId)}
                                            >
                                                <div className="habit-option-name">{habit.name}</div>
                                                {habit.description && (
                                                    <div className="habit-option-desc">{habit.description}</div>
                                                )}
                                            </button>
                                        ))}
                                    </div>
                                )}
                            </div>
                        </div>
                    )}

                    
                    <div className="form-group">
                        <label>Дата выполнения</label>
                        <input
                            type="date"
                            value={executeDate}
                            onChange={(e) => setExecuteDate(e.target.value)}
                            required
                        />
                    </div>

                    <div className="modal-actions">
                        <button
                            type="button"
                            className="btn-cancel"
                            onClick={onClose}
                            disabled={submitting}
                        >
                            Отмена
                        </button>
                        <button
                            type="submit"
                            className="btn-submit"
                            disabled={submitting}
                        >
                            {submitting ? 'Сохранение...' : 'Добавить'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}


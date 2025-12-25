import { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createSession } from '../api/sessionApi';
import Header from './Header';
import './style/BreathingPractice.css';

const TECHNIQUES = {
    box: {
        key: 'box',
        label: 'Квадратное дыхание 4-4-4-4',
        phases: [
            { label: 'Вдох', seconds: 4, phaseKey: 'inhale' },
            { label: 'Задержка', seconds: 4, phaseKey: 'holdAfterInhale' },
            { label: 'Выдох', seconds: 4, phaseKey: 'exhale' },
            { label: 'Задержка', seconds: 4, phaseKey: 'holdAfterExhale' }
        ]
    },
    four78: {
        key: 'four78',
        label: 'Техника 4-7-8',
        phases: [
            { label: 'Вдох', seconds: 4, phaseKey: 'inhale' },
            { label: 'Задержка', seconds: 7, phaseKey: 'hold' },
            { label: 'Выдох', seconds: 8, phaseKey: 'exhale' }
        ]
    }
};

const formatSeconds = (value) => value.toString().padStart(2, '0');

export default function BreathingPractice() {
    const navigate = useNavigate();
    const [techniqueKey, setTechniqueKey] = useState('box');
    const [phaseIndex, setPhaseIndex] = useState(0);
    const [remaining, setRemaining] = useState(TECHNIQUES.box.phases[0].seconds);
    const remainingRef = useRef(remaining);
    const [isRunning, setIsRunning] = useState(false);
    const [cycles, setCycles] = useState(0);
    const [elapsed, setElapsed] = useState(0); 
    const [startedAt, setStartedAt] = useState(null);
    const timerRef = useRef(null);
    const [saving, setSaving] = useState(false);
    const [phaseProgress, setPhaseProgress] = useState(0); 
    const [userName, setUserName] = useState('');

    const technique = useMemo(() => TECHNIQUES[techniqueKey], [techniqueKey]);
    const currentPhase = technique.phases[phaseIndex];
    const phaseIndexRef = useRef(phaseIndex);
    const techniqueRef = useRef(technique);

    useEffect(() => {
        remainingRef.current = remaining;
    }, [remaining]);

    useEffect(() => {
        
        setPhaseIndex(0);
        setRemaining(technique.phases[0].seconds);
        setIsRunning(false);
        setPhaseProgress(0);
        clearInterval(timerRef.current);
        phaseIndexRef.current = 0;
        techniqueRef.current = technique;
    }, [techniqueKey, technique]);

    useEffect(() => {
        const user = localStorage.getItem('user');
        if (user) {
            try {
                const parsed = JSON.parse(user);
                setUserName(parsed.nickname || parsed.email || '');
            } catch {
                setUserName('');
            }
        }
    }, []);

    useEffect(() => {
        if (!isRunning) {
            clearInterval(timerRef.current);
            timerRef.current = null;
            return;
        }

        const TICK_MS = 50;
        timerRef.current = setInterval(() => {
            const delta = TICK_MS / 1000;
            setElapsed((e) => e + delta);

            const tech = techniqueRef.current;
            const idx = phaseIndexRef.current;
            const phase = tech.phases[idx];

            setRemaining((prev) => {
                const nextVal = prev - delta;
                const phaseDuration = phase.seconds;
                const progress = Math.min(1, Math.max(0, (phaseDuration - Math.max(nextVal, 0)) / phaseDuration));
                setPhaseProgress(progress);

                if (nextVal > 0) return nextVal;

               
                const nextIndex = (idx + 1) % tech.phases.length;
                phaseIndexRef.current = nextIndex;
                setPhaseIndex(nextIndex);
                if (nextIndex === 0) setCycles((c) => c + 1);
                setPhaseProgress(0);
                const nextPhaseDuration = tech.phases[nextIndex].seconds;
                remainingRef.current = nextPhaseDuration;
                return nextPhaseDuration;
            });
        }, TICK_MS);

        return () => {
            clearInterval(timerRef.current);
        };
    }, [isRunning]);

    const handleStart = () => {
        if (isRunning) return;
        setStartedAt(new Date());
        setElapsed(0);
        setCycles(0);
        setPhaseIndex(0);
        setRemaining(technique.phases[0].seconds);
        phaseIndexRef.current = 0;
        techniqueRef.current = technique;
        setPhaseProgress(0);
        setIsRunning(true);
    };

    const handlePauseToggle = () => {
        setIsRunning((v) => !v);
    };

    const handleReset = () => {
        setIsRunning(false);
        setElapsed(0);
        setCycles(0);
        setPhaseIndex(0);
        setRemaining(technique.phases[0].seconds);
        phaseIndexRef.current = 0;
        setPhaseProgress(0);
        setStartedAt(null);
    };

    const handleComplete = async () => {
        if (!startedAt || elapsed === 0) {
            alert('Сначала запустите и выполните сессию.');
            return;
        }
        setSaving(true);
        try {
            await createSession({
                type: technique.label,
                duration: Math.round(elapsed),
                completed: true,
                notes: null,
                startedAt: startedAt.toISOString()
            });
            alert('Сессия сохранена.');
            handleReset();
        } catch (err) {
            alert(err.response?.data?.error || err.response?.data || 'Не удалось сохранить сессию');
        } finally {
            setSaving(false);
        }
    };

    const currentPhaseClass = currentPhase?.phaseKey ? `phase-${currentPhase.phaseKey}` : '';

    const circleScale = (() => {
        const minScale = 0.85;
        const maxScale = 1.15;

        switch (currentPhase?.phaseKey) {
            case 'inhale':
                
                return minScale + (maxScale - minScale) * phaseProgress;
            case 'holdAfterInhale':
                
                return maxScale;
            case 'exhale':
                
                return maxScale - (maxScale - minScale) * phaseProgress;
            case 'holdAfterExhale':
             
                return minScale;
            default:
                return 1;
        }
    })();

    const circleStyle = {
        transform: `scale(${circleScale})`,
    };

    return (
        <div className="breath-page">
            <div className="breath-content">
                <button className="back-button" onClick={() => navigate('/app')}>
                    ← Назад к приложениям
                </button>
                <header className="breath-header">
                    <h1>Дыхательная практика</h1>
                    <p>Выберите технику и следуйте подсказкам таймера.</p>
                </header>

                <section className="breath-controls">
                    <label>
                        Техника:
                        <select
                            value={techniqueKey}
                            onChange={(e) => setTechniqueKey(e.target.value)}
                            disabled={isRunning}
                        >
                            {Object.values(TECHNIQUES).map((t) => (
                                <option key={t.key} value={t.key}>
                                    {t.label}
                                </option>
                            ))}
                        </select>
                    </label>

                    <div className="button-row">
                        <button onClick={handleStart} disabled={isRunning}>
                            Старт
                        </button>
                        <button onClick={handlePauseToggle} disabled={!startedAt}>
                            {isRunning ? 'Пауза' : 'Продолжить'}
                        </button>
                        <button onClick={handleReset} disabled={!startedAt}>
                            Сброс
                        </button>
                        <button onClick={handleComplete} disabled={!startedAt || saving}>
                            {saving ? 'Сохранение...' : 'Завершить и сохранить'}
                        </button>
                    </div>
                </section>

                <section className="breath-widget">
                    <div className={`breath-visual ${currentPhaseClass}`}>
                        <div className="breath-visual-circle" style={circleStyle} />
                    </div>
                    <div className="breath-info">
                        <p className="phase-label">{currentPhase?.label}</p>
                        <p className="timer">
                            {formatSeconds(Math.max(0, Math.ceil(remaining)))} сек.
                        </p>
                        <div className="meta">
                            <span>Циклов: {cycles}</span>
                            <span>Всего: {Math.round(elapsed)} сек.</span>
                        </div>
                    </div>
                </section>
            </div>
            <Header />
        </div>
    );
}


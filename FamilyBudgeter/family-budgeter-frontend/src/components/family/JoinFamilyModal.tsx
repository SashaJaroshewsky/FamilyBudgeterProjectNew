import React, { useState } from 'react';
import { Modal, Form, Button, Alert } from 'react-bootstrap';
import { familyApi } from '../../api/familyApi';

interface JoinFamilyModalProps {
  show: boolean;
  onHide: () => void;
  onSuccess: () => void;
}

const JoinFamilyModal: React.FC<JoinFamilyModalProps> = ({ show, onHide, onSuccess }) => {
  const [joinCode, setJoinCode] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!joinCode.trim()) {
      setError('Будь ласка, введіть код приєднання');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      await familyApi.joinFamily({ joinCode: joinCode.trim() });
      onSuccess();
      onHide();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Помилка приєднання до сім\'ї');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal show={show} onHide={onHide} centered>
      <Modal.Header closeButton>
        <Modal.Title>Приєднатися до сім'ї</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {error && (
          <Alert variant="danger" className="mb-3" dismissible onClose={() => setError(null)}>
            {error}
          </Alert>
        )}
        <Form onSubmit={handleSubmit}>
          <Form.Group className="mb-3">
            <Form.Label>Код приєднання</Form.Label>
            <Form.Control
              type="text"
              placeholder="Введіть код приєднання"
              value={joinCode}
              onChange={(e) => setJoinCode(e.target.value)}
              disabled={loading}
            />
            <Form.Text className="text-muted">
              Введіть код, який вам надав адміністратор сім'ї
            </Form.Text>
          </Form.Group>
          <div className="d-flex justify-content-end gap-2">
            <Button variant="secondary" onClick={onHide} disabled={loading}>
              Скасувати
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? (
                <>
                  <span className="spinner-border spinner-border-sm me-2" />
                  Приєднання...
                </>
              ) : (
                'Приєднатися'
              )}
            </Button>
          </div>
        </Form>
      </Modal.Body>
    </Modal>
  );
};

export default JoinFamilyModal;
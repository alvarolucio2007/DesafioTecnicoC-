import { useState, useEffect } from 'react';
import axios from 'axios';

const API_URL = 'http://localhost:5262/api/pessoa';

export default function App() {
  const [pessoas, setPessoas] = useState([]);
  const [nome, setNome] = useState('');
  const [idade, setIdade] = useState('');
  const [idEdicao, setIdEdicao] = useState(null); // Controla se estamos editando alguém
  const [erro, setErro] = useState('');

  // Carrega a lista de pessoas assim que a página abre
  useEffect(() => {
    carregarPessoas();
  }, []);

  const carregarPessoas = async () => {
    try {
      const res = await axios.get(API_URL);
      setPessoas(res.data);
    } catch (err) {
      setErro('Erro ao carregar a lista de pessoas.');
    }
  };

  // Envia os dados (Salva um novo ou Atualiza um existente)
  const salvarPessoa = async (e) => {
    e.preventDefault();
    setErro('');

    const payload = { nome, idade: Number(idade) };

    try {
      if (idEdicao) {
        // Se tem idEdicao, envia PUT para atualizar
        await axios.put(`${API_URL}/${idEdicao}`, payload);
        setIdEdicao(null);
      } else {
        // Se não tem, envia POST para criar
        await axios.post(API_URL, payload);
      }

      // Limpa os campos e recarrega a tabela
      setNome('');
      setIdade('');
      carregarPessoas();
    } catch (err) {
      setErro(err.response?.data || 'Erro ao salvar os dados.');
    }
  };

  // Prepara o formulário com os dados da pessoa para edição
  const iniciarEdicao = (pessoa) => {
    setIdEdicao(pessoa.id);
    setNome(pessoa.nome);
    setIdade(pessoa.idade);
  };

  // Cancela o modo de edição
  const cancelarEdicao = () => {
    setIdEdicao(null);
    setNome('');
    setIdade('');
  };

  // Deleta a pessoa do banco
  const deletarPessoa = async (id) => {
    if (!confirm('Deseja realmente excluir esta pessoa?')) return;

    try {
      await axios.delete(`${API_URL}/${id}`);
      carregarPessoas();
    } catch (err) {
      setErro(err.response?.data || 'Erro ao deletar pessoa.');
    }
  };

  return (
    <div style={{ padding: '30px', fontFamily: 'sans-serif', maxWidth: '800px', margin: '0 auto' }}>
      <h1>Gerenciamento de Pessoas</h1>

      {erro && <p style={{ color: 'red', fontWeight: 'bold' }}>{erro}</p>}

      {/* FORMULÁRIO */}
      <form onSubmit={salvarPessoa} style={{ display: 'flex', gap: '10px', marginBottom: '30px', alignItems: 'center' }}>
        <input
          type="text"
          placeholder="Nome"
          value={nome}
          onChange={e => setNome(e.target.value)}
          required
        />
        <input
          type="number"
          placeholder="Idade"
          value={idade}
          onChange={e => setIdade(e.target.value)}
          required
        />
        <button type="submit" style={{ backgroundColor: idEdicao ? '#ffc107' : '#28a745', color: '#fff', border: 'none', padding: '8px 15px', cursor: 'pointer' }}>
          {idEdicao ? 'Atualizar' : 'Cadastrar'}
        </button>
        {idEdicao && (
          <button type="button" onClick={cancelarEdicao} style={{ backgroundColor: '#6c757d', color: '#fff', border: 'none', padding: '8px 15px', cursor: 'pointer' }}>
            Cancelar
          </button>
        )}
      </form>

      {/* TABELA DE EXIBIÇÃO */}
      <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
        <thead>
          <tr style={{ backgroundColor: '#f2f2f2' }}>
            <th>ID</th>
            <th>Nome</th>
            <th>Idade</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {pessoas.length === 0 ? (
            <tr>
              <td colSpan="4" style={{ textAlign: 'center' }}>Nenhuma pessoa cadastrada.</td>
            </tr>
          ) : (
            pessoas.map(p => (
              <tr key={p.id}>
                <td>{p.id}</td>
                <td>{p.nome}</td>
                <td>{p.idade}</td>
                <td>
                  <button onClick={() => iniciarEdicao(p)} style={{ marginRight: '10px', cursor: 'pointer' }}>Editar</button>
                  <button onClick={() => deletarPessoa(p.id)} style={{ color: 'red', cursor: 'pointer' }}>Excluir</button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}

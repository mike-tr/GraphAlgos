namespace MGraph {
    public interface IEdgeReference {
        void Subscribe (Edge edge);
        bool AddEdge (int n1, int n2);
    }
}

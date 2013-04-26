import java.io.File;
import java.io.FileNotFoundException;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;

public class ThreeLetterWord {
	 Set<String> dictionary = new HashSet<String>();
	 Scanner scanner;
	 Scanner inputScanner;
	 ConcurrentHashMap<String, Node> graph;
	 static String alphabet = "abcdefghijklmnopqrstuvwxyz";
	 class Node {
		 String word;
		 Node predecessor;
		 ArrayList<Node> nodeList;
		 int depth;
		 boolean visited;
		 public Node(String s) {
			 word = s;
			 predecessor = null;
			 depth = -1;
			 visited = false;
		 }
		 public void setPredecessor(Node n) {
			 predecessor = n;
		 }
		 public void setList(ArrayList<Node> a) {
			 nodeList = a;
		 }
		 public ArrayList<Node> getList() {
			 return nodeList;
		 }
		 public Node getPredecessor() {
			 return predecessor;
		 }
		 public void setDepth(int i) {
			 depth = i;
		 }
		 public int getDepth() {
			 return depth;
		 }
		 public void setVisited() {
			 visited = true;
		 }
		 public boolean getVisited() {
			 return visited;
		 }
	 }
	 public ThreeLetterWord() {
		
		try {
			scanner = new Scanner(new File("three-letter-words.txt"));
		} catch(FileNotFoundException e) {
			System.out.println("Couldn't find file three-letter-words.txt");
		}
		while (scanner.hasNext()) {
			dictionary.add(scanner.nextLine().toLowerCase());
		}
		inputScanner = new Scanner(System.in);
		
		 String start, end;
		 while(true) {
			 graph = new ConcurrentHashMap<String, Node>();
			 System.out.println("Enter starting word: ");
			 start = inputScanner.nextLine();
			 System.out.println("Enter ending word: ");
			 end = inputScanner.nextLine();
			 makeGraph(graph, start);
			breadthfirstsearch(graph, start, end);
			System.out.println("");
		 }
	 }
	 public static void main(String[] args) {
		ThreeLetterWord tlw = new ThreeLetterWord();
		 }
	 public void breadthfirstsearch(ConcurrentHashMap<String, Node> graph, String start, String end) {
		 ArrayDeque<Node> queue = new ArrayDeque<Node>();
		 ArrayList<Node> nodeList;
		 String temp;
		 boolean found = false;
		 queue.add(graph.get(start));
		 looking:
		 while(!queue.isEmpty()) {
			 temp = queue.pop().word;
			 if (!graph.get(temp).getVisited()) {
				 if (temp.equals(end)) {
					 queue.clear();
					 break;
				 }
			 }
				 
						 nodeList = makeGraph(graph, temp);
				 for(Node n : nodeList) {
					 if (!graph.get(n.word).getVisited()) {
						 graph.get(n.word).setPredecessor(graph.get(temp));
						 graph.get(n.word).setVisited();
						 queue.add(graph.get(n.word));
						 if (n.word.equals(end)) {
							 queue.clear();
							 break looking;
						 }
					 }
				 }
			 
		 }
		 if (graph.get(end)==null) {
			 System.out.println("Could not find match to " + end);
		 }
		 else {
			 System.out.print(start + " -> ");
			 String from = end;
			 Node endNode = graph.get(from);
		     while (graph.get(from).getPredecessor()!=null) {
		    	 if (!graph.get(from).getPredecessor().word.equals(start)) {
		    		 queue.add(graph.get(from).getPredecessor());
		    	 }
		    	 else
		    		 break;
		    	 from = graph.get(from).getPredecessor().word;
		    }
		     while (!queue.isEmpty()) {
		         System.out.print(queue.pollLast().word + " -> ");
		     }
		     System.out.print(end);
		 }
	 }
	 public void mapNeighbors(String word) {
		ArrayList<Node> nodes = new ArrayList<Node>();
		Node temp;
		for (String s : findNeighbors(word)) {
			nodes.add(new Node(s));
		}
		temp = new Node(word);
		temp.setList(nodes);
	 }
	 
	 public ArrayList<String> findNeighbors(String word) {
		 ArrayList<String> neighbors = new ArrayList<String>();
		 StringBuilder theWord = new StringBuilder(word);
		 StringBuilder newWord;
		 for (int i=0; i<3; i++) {
			 for (int j=0; j<26; j++) {
				 if (alphabet.charAt(j)!=theWord.charAt(i)) {
					 newWord = new StringBuilder(word);
					 newWord.setCharAt(i, alphabet.charAt(j));
					 if (dictionary.contains(newWord.toString())) {
						 neighbors.add(newWord.toString());
					 }
				 }
			 }
		 }
		 return neighbors;
	 }
 
	 public ArrayList<Node> makeGraph(ConcurrentHashMap<String, Node> graph, String word) {
		 ArrayList<String> neighbors = findNeighbors(word);
		 ArrayList<Node> nodes = new ArrayList<Node>();
		 Node startNode = new Node(word);
		 Node newNode;
		 for (String s : neighbors) { //change string list to node list
			 newNode = new Node(s);
			 nodes.add(newNode);
			 graph.putIfAbsent(s, newNode);
		 }
		 startNode.setList(nodes);
		 graph.putIfAbsent(word, startNode);
		 graph.get(word).setList(nodes);
		 return nodes;
	 }
}

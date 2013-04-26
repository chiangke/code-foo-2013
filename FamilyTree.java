import java.util.*;

/*This algorithm for a family tree of n members has runtime of theta(2n)
 *Upon creating the family tree (a linked list where each member has a linked list to all its
 *children), the program runs through every member and hashes it with his/her generation and 
 *first name. 
 *Now that all members are hashed to buckets with keys being generation or first name, the
 *lookup complexity is simply O(1). 
 *Because we want all members that match name/generation, the issue of collisions does not apply
 *and there is no need to search through the hash bucket; we can simply grab all members in 
 *that bucket and return it.
 *
 *Algorithm scales linearly with the number of members in the tree.
 *Because two hashes are done (one for generation, one for first name), 
 *the complexity is theta(2n)
 */

public class FamilyTree {
	static FamilyMember james, joe, zoey, daniel, erika, robert, pat, mike, chad, mark;
	public FamilyTree() {
		james = new FamilyMember("James A", 1);
		joe = new FamilyMember("Joe A", 2);
		james.addChild(joe);
		zoey = new FamilyMember("Zoey A", 2);
		james.addChild(zoey);
		daniel = new FamilyMember("Daniel P", 3);
		zoey.addChild(daniel);
		erika = new FamilyMember("Erika A", 3);
		joe.addChild(erika);
		robert = new FamilyMember("Robert A", 3);
		joe.addChild(robert);
		pat = new FamilyMember("Pat P", 4);
		daniel.addChild(pat);
		mike = new FamilyMember("Mike M", 4);
		erika.addChild(mike);
		chad = new FamilyMember("Chad A", 4);
		robert.addChild(chad);
		mark = new FamilyMember("Mark A", 4);
		robert.addChild(mark);
		
	}
	public static void main(String[] args) {
		FamilyTree ft = new FamilyTree();
		ArrayList<FamilyMember> allMembers = getAllMembers();
		HashMap<Integer, List<FamilyMember>> genMap = hashGen(allMembers);
		HashMap<String, List<FamilyMember>> nameMap = hashName(allMembers);
		for(FamilyMember m : genMap.get(4)) {
			System.out.print(m.getName()+" ");
		}
		System.out.println("");
		for(FamilyMember m : nameMap.get("Mark")) {
			System.out.print(m.getName()+" ");
		}
		
		/*ArrayList<FamilyMember> children = james.getChildren();
		System.out.println("James' children:");
		for(FamilyMember m : children) {
			System.out.print(m.getName() + " ");
		}
		System.out.println("");
		ArrayList<FamilyMember> generation = getGeneration(4);
		System.out.println("Generation 4 members:");
		if (generation!=null) {
			for(FamilyMember m : generation) {
				System.out.print(m.getName()+" ");
			}
		}
		else {
			System.out.println("No members of this generation exist");
		}
		System.out.println("");
		ArrayList<FamilyMember> name = getName("Chad");
		System.out.println("Members named Chad:");
		if (name.size()>0) {
			for(FamilyMember m : name) {
				System.out.print(m.getName()+" ");
			}
		}
		else {
			System.out.println("No members of this name exist");
		}*/
	}
	static public ArrayList<FamilyMember> getGeneration(int g) {
		FamilyMember current = james;
		ArrayList<FamilyMember> goTo = new ArrayList<FamilyMember>();
		if (g == 1) {
			goTo.add(james);
			return goTo;
		}
		goTo.addAll(current.getChildren());
		int numSize = goTo.size();
		current = goTo.get(0);
		while(current.getGeneration() < g) {
			for(int j=0; j<numSize; j++) {
				goTo.addAll(current.getChildren());
				goTo.remove(0);
				try {
					current = goTo.get(0);
				}
				catch (IndexOutOfBoundsException e) {
					return null;
				}
			}
			//current = goTo.get(0);
			numSize = goTo.size();
		}
		return goTo;
	}
	/*static public ArrayList<FamilyMember> getName(String n) {
		int test = 1;
		ArrayList<FamilyMember> allMembers = new ArrayList<FamilyMember>();
		ArrayList<FamilyMember> nameMatch = new ArrayList<FamilyMember>();
		while (getGeneration(test)!=null) {
			allMembers.addAll(getGeneration(test));
			test++;
		}
		for (FamilyMember m: allMembers) {
			if(m.getName().substring(0, m.getName().indexOf(' ')).equals(n)) {
				nameMatch.add(m);
			}
		}
		return nameMatch;
		
	}*/
	static public ArrayList<FamilyMember> getAllMembers() {
		ArrayList<FamilyMember> allMembers = new ArrayList<FamilyMember>();
		int test = 1;
		while (getGeneration(test)!=null) {
			allMembers.addAll(getGeneration(test));
			test++;
		}
		return allMembers;
	}
	static public HashMap hashGen(ArrayList<FamilyMember> a) {
		HashMap<Integer, ArrayList<FamilyMember>> genMap = new HashMap<Integer, ArrayList<FamilyMember>>();
		ArrayList<FamilyMember> tempList;
		for (FamilyMember f : a) {
			if (!genMap.containsKey(f.getGeneration())) {
				tempList = new ArrayList<FamilyMember>();
			}
			else {
				tempList = genMap.get(f.getGeneration());
			}
			tempList.add(f);
			genMap.put(f.getGeneration(), tempList);
		}
		return genMap;
	}
	static public HashMap hashName(ArrayList<FamilyMember> a) {
		HashMap<String, ArrayList<FamilyMember>> nameMap = new HashMap<String, ArrayList<FamilyMember>>();
		ArrayList<FamilyMember> tempList;
		for (FamilyMember f : a) {
			String firstName = f.getName().substring(0, f.getName().indexOf(' '));
			if (!nameMap.containsKey(firstName)) {
				tempList = new ArrayList<FamilyMember>();
			}
			else {
				tempList = nameMap.get(firstName);
			}
			tempList.add(f);
			nameMap.put(firstName, tempList);
		}
		return nameMap;
	}
}

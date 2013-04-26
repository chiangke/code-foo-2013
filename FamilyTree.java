import java.util.*;

/*This algorithm for a family tree of n members has a total runtime of theta(2n)
 *Upon creating the family tree (a linked list where each member has a linked list to all its
 *children), the program runs through every member and hashes it with his/her generation and 
 *first name. 
 *Now that all members are hashed to buckets with keys being the generation or first name, the
 *lookup complexity is simply O(1). 
 *Because we want all members that match name/generation, the issue of collisions does not apply
 *and there is no need to search through the hash bucket; we can simply grab all members in 
 *that bucket and return the linkedlist.
 *
 *Algorithm scales linearly with the number of members n in the tree.
 *Because two hashes are done (one for generation, one for first name), 
 *the time complexity to hash is simply theta(2n), while lookup is O(1).
 */

public class FamilyTree {
	static FamilyMember james, joe, zoey, daniel, erika, robert, pat, mike, chad, marka, markb, alvin;
	public FamilyTree() {
		//construct test family tree with 4 generations
		james = new FamilyMember("James A", 1); //(name, generation)
		joe = new FamilyMember("Joe A", 2); 
		james.addChild(joe);  //set joe as child of james
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
		marka = new FamilyMember("Mark A", 4);
		robert.addChild(marka);
		alvin = new FamilyMember("Alvin T", 4);
		erika.addChild(alvin);
		markb = new FamilyMember("Mark B", 5);
		marka.addChild(markb);
		
		ArrayList<FamilyMember> allMembers = getAllMembers();
		HashMap<Integer, List<FamilyMember>> genMap = hashGen(allMembers);
		HashMap<String, List<FamilyMember>> nameMap = hashName(allMembers);
		
		//retrieve matches
		System.out.println("All members of generation 4: ");
		for(FamilyMember m : genMap.get(4)) {
			System.out.println(m.getName()+" ");
		}
		
		System.out.println("\nAll members with first name Mark: ");
		for(FamilyMember m : nameMap.get("Mark")) {
			System.out.println(m.getName()+" ");
		}
		
	}
	public static void main(String[] args) {
		FamilyTree ft = new FamilyTree();
	}
	public ArrayList<FamilyMember> getGeneration(int g) {
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
			numSize = goTo.size();
		}
		return goTo;
	}
	public ArrayList<FamilyMember> getAllMembers() {
		ArrayList<FamilyMember> allMembers = new ArrayList<FamilyMember>();
		int test = 1;
		while (getGeneration(test)!=null) {
			allMembers.addAll(getGeneration(test));
			test++;
		}
		return allMembers;
	}
	public HashMap hashGen(ArrayList<FamilyMember> a) {
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
	public HashMap hashName(ArrayList<FamilyMember> a) {
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

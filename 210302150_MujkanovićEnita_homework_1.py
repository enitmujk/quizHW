#importing/reading a .json file into my Python code
import json
import random

def ask_question(question):
    print(question['question'])
    for i, option in enumerate(question['options'], start=1):
        print(f"{i}. {option}")
    
    while True:
        try:
            user_answer = int(input("Your answer (enter the number corresponding to your choice): "))
            if 1 <= user_answer <= len(question['options']):
                return user_answer
            else:
                print("Invalid choice. Please enter a number between 1 and", len(question['options']))
        except ValueError:
            print("Invalid input. Please enter a number.")


#open the file in read mode using a relative path

file_path = "pitanja.json"
with open(file_path, 'r') as file:
    data = json.load(file)  #loads the .json data from the file

#validated the structure of each qs
questions = data['Quiz']['questions'] #extracts the list of qs from the loaded data
for idx, question in enumerate(questions, start=1):
    if not all(key in question for key in ['question', 'options', 'answer']):
        raise ValueError(f"Invalid structure for question {idx}. Each question must have 'question', 'options', and 'answer' keys.")
    

# num_random_questions = 5 #num of random qs I want to select
num_questions_to_ask = len(questions)

random_questions = random.sample(questions, num_questions_to_ask) 
#randomly selects qs

#for idx, question in enumerate(random_questions, start=1):
#    print(f"\nQuestion {idx}: {question['question']}")
#    for i, option in enumerate(question['options'], start=1):
#        print(f"{i}. {option}")
#    print("")
#this one displays the randomly selected qs    

#quiz functionality
correct_answers = 0
for idx, question in enumerate(random_questions, start=1):
    input("Press enter to start the next question!")

    user_choice = ask_question(question)
    correct_choice = question['options'].index(question['answer']) + 1
    if user_choice == correct_choice:
        print("Correct!\n")
        correct_answers += 1
    else:
        print(f"Incorrect. The correct answer is {correct_choice}: {question['answer']}\n")

# Display quiz results
print(f"You answered {correct_answers} out of {num_questions_to_ask} questions correctly.")


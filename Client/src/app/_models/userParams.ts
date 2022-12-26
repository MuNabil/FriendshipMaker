import { User } from "./user";

// This class To send the query string params
// It's a class so I can give the properities an intial value
export class UserParams {
    pageNumber = 1;
    pageSize = 5;
    minAge = 18;
    maxAge = 80;
    gender: string;
    orderBy = 'lastActive';

    // To also set the intial value for gender as well
    constructor(user: User) {
        this.gender = user.gender === 'male' ? 'female' : 'male';
    }
}
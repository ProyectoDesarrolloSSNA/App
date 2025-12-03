
export interface CreateUpdateRatingDto {
  destinationId?: string;
  userId?: string;
  stars: number;
  comment?: string;
}

export interface RatingDto {
  id?: string;
  destinationId?: string;
  userId?: string;
  stars: number;
  comment?: string;
}
